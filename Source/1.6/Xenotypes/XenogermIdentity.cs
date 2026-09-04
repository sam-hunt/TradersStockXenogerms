using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace XenogermTraderStock
{
    // What a xenogerm implants as, when this mod has anything to say about it: a
    // preset XenotypeDef, or an inheritable player-scenario CustomXenotype whose
    // germline the implant patch will write. Neither set (None) means vanilla's
    // implant is already the whole story - a non-inheritable custom template, an
    // assembler germ matching nothing, or an unbreakable tie.
    public readonly struct XenogermSource
    {
        public static readonly XenogermSource None = default;

        public readonly XenotypeDef Preset;
        public readonly CustomXenotype Custom;

        private XenogermSource(XenotypeDef preset, CustomXenotype custom)
        {
            Preset = preset;
            Custom = custom;
        }

        public static XenogermSource Of(XenotypeDef preset) => new XenogermSource(preset, null);
        public static XenogermSource Of(CustomXenotype custom) => new XenogermSource(null, custom);

        public bool IsNone => Preset == null && Custom == null;

        // Whether the xenotype lives in the germline - the retarget's precondition.
        // A None source is never inheritable.
        public bool Inheritable => Preset != null ? Preset.inheritable : Custom?.inheritable == true;
    }

    // Which xenotype does this xenogerm produce? The single answer read by the
    // implant stamp (GeneUtility_ImplantXenogermItem_Patch) and the ideology gate
    // (Xenogerm_PawnIdeoDisallowsImplanting_Patch). Pricing deliberately does NOT
    // use it - see the StatParts.
    //
    // Two sources, in order:
    //
    //  1. CompXenotypeSource.sourceXenotype - stamped by XenogermFactory on every
    //     trader-sold preset germ. Authoritative when present. (The comp's
    //     sourceCustomName is NOT authoritative: a custom template is identified
    //     by gene set, below, and the name only breaks ties.)
    //
    //  2. Gene-set inference. Comp state does not survive every path an item can
    //     take: ReSplice Core's xenogerm duplicator builds its copy with
    //     ThingMaker.MakeThing and copies only xenotypeName, iconDef and the gene
    //     list, so the copy of a Hussar germ has a null comp and would implant as a
    //     custom xenotype merely named "hussar" - same genes, no ideology
    //     recognition, no germline retarget. Any other cloner that copies fields
    //     rather than comps loses the def the same way. So a comp-less germ whose
    //     genes equal exactly one preset's gene list IS that preset. This is
    //     vanilla's own notion of identity - Pawn_GeneTracker.CustomXenotype and
    //     GeneUtility.PawnIsCustomXenotype resolve a pawn's custom xenotype by gene
    //     match and never by name - extended from custom templates to preset defs.
    //     A side effect by design: a player who assembles exactly Hussar's genes at
    //     the gene assembler gets a Hussar, whatever they typed in the name box.
    //
    // Inference only names what vanilla cannot, and the player's own templates
    // come first. Before any preset is considered, the game's custom xenotype
    // database (Current.Game.customXenotypeDatabase - the templates the player
    // authored in the xenotype editor, seeded from the scenario's starting pawns
    // and the chosen ideos' xenotype precepts) gets first claim on the germ's
    // gene set:
    //
    //  * A NON-inheritable template that matches claims the germ and the resolver
    //    returns None: vanilla's implant runs untouched and the pawn's
    //    CustomXenotype resolves to that template exactly as it would without this
    //    mod. A player who went to the trouble of authoring a preset clone and not
    //    picking the preset has expressed a preference, whatever the reason (an
    //    interaction with some other mod, say), and this mod should not overrule
    //    it.
    //
    //  * An INHERITABLE template that matches is returned as the source, because
    //    here vanilla cannot deliver the template at all: PawnIsCustomXenotype
    //    matches an inheritable template against ENDOgenes, which a xenogene
    //    implant never writes, so a vanilla implant of the template's own genes
    //    yields a pawn labeled with its name that the game does not recognise as
    //    the template, whose children inherit nothing. The implant patch writes the
    //    germline instead (the same retarget an inheritable preset gets), after
    //    which the pawn IS the template by vanilla's own test - ideo preference and
    //    inheritance included. This is the mod's flagship behaviour applied to the
    //    scenario xenotypes it sells; without it, the trader-sold germ of a
    //    colony's own germline xenotype produced xenogene copies that could never
    //    breed true.
    //
    // The comp path is unaffected - a trader-sold preset germ stamps its def
    // regardless - so a bought preset germ and its comp-less copy can diverge in a
    // game holding a template clone of that preset; that is the player's template
    // winning, by design.
    //
    // The germ's xenotypeName (or the comp's stored custom name) is a tie-breaker
    // only. Two templates, or two loaded presets, sharing one gene list (a mod
    // re-shipping a vanilla xenotype under its own defName) are split by comparing
    // the name to each candidate's name / label / defName; a tie the name cannot
    // break resolves to None and the germ implants as vanilla would. Name-first
    // matching was rejected: a player working towards a preset one genepack at a
    // time ("hussar", "hussar2", "hussar3") ends with a germ that is a Hussar in
    // every way vanilla can measure, and the name they happened to type last
    // should not decide that.
    //
    // Never infers a gene-less xenotype. Baseliner's empty germ is the
    // baseliner-conversion item - its implant wipes both gene layers - and it is
    // only ever created with the comp set (XenogermFactory / the debug spawner).
    // Vanilla cannot produce an empty xenogerm, but a modded or malformed one must
    // not turn into a germline wipe by accident.
    public static class XenogermIdentity
    {
        public static XenogermSource Resolve(Xenogerm xenogerm)
        {
            if (xenogerm == null)
            {
                return XenogermSource.None;
            }

            CompXenotypeSource comp = xenogerm.TryGetComp<CompXenotypeSource>();
            if (comp?.sourceXenotype != null)
            {
                return XenogermSource.Of(comp.sourceXenotype);
            }

            GeneSet geneSet = xenogerm.GeneSet;
            if (geneSet == null)
            {
                return XenogermSource.None;
            }

            string name = comp?.sourceCustomName.NullOrEmpty() == false
                ? comp.sourceCustomName
                : xenogerm.xenotypeName;

            return Infer(
                geneSet.GenesListForReading,
                name,
                DefDatabase<XenotypeDef>.AllDefsListForReading,
                Current.Game?.customXenotypeDatabase?.customXenotypes);
        }

        // Pure over its inputs so the headless suite can cover it. Custom templates
        // first: the one whose genes match the germ's (see GenesMatch), or with
        // several the one whose name equals xenotypeName (case-insensitive) - an
        // inheritable match is the source, a non-inheritable one claims the germ
        // for vanilla (None), an unbreakable tie is None. Only when no template
        // matches: the one preset whose genes match, ties broken against label /
        // defName the same way, otherwise None.
        public static XenogermSource Infer(
            List<GeneDef> germGenes,
            string xenotypeName,
            IEnumerable<XenotypeDef> candidates,
            IEnumerable<CustomXenotype> customTemplates = null)
        {
            if (germGenes == null || germGenes.Count == 0)
            {
                return XenogermSource.None;
            }

            string name = xenotypeName?.Trim();

            if (customTemplates != null)
            {
                bool claimed = TryPickMatch(customTemplates, germGenes,
                    c => c?.genes, c => NameMatches(c, name), out CustomXenotype template);
                if (claimed)
                {
                    // A template matched (or several tied): the player's own xenotype
                    // decides. Inheritable means the retarget can deliver it; anything
                    // else - non-inheritable, or a tie the name did not break - is
                    // vanilla's to handle.
                    return template?.inheritable == true
                        ? XenogermSource.Of(template)
                        : XenogermSource.None;
                }
            }

            if (candidates == null)
            {
                return XenogermSource.None;
            }

            TryPickMatch(candidates, germGenes, d => d?.genes, d => NameMatches(d, name), out XenotypeDef preset);
            return preset != null ? XenogermSource.Of(preset) : XenogermSource.None;
        }

        // Shared match-then-tie-break over either candidate kind. Returns whether
        // ANY candidate's genes matched (so a caller can tell "claimed but
        // unresolved" from "nothing matched"); `picked` is the single match, or
        // the single name-matching candidate among several, else null.
        private static bool TryPickMatch<T>(
            IEnumerable<T> candidates,
            List<GeneDef> germGenes,
            Func<T, List<GeneDef>> genesOf,
            Func<T, bool> nameMatches,
            out T picked)
            where T : class
        {
            T single = null;
            List<T> ties = null;
            foreach (T candidate in candidates)
            {
                List<GeneDef> genes = genesOf(candidate);
                if (genes == null || genes.Count == 0 || !GenesMatch(germGenes, genes))
                {
                    continue;
                }

                if (single == null)
                {
                    single = candidate;
                }
                else
                {
                    ties ??= new List<T> { single };
                    ties.Add(candidate);
                }
            }

            if (single == null)
            {
                picked = null;
                return false;
            }

            if (ties == null)
            {
                picked = single;
                return true;
            }

            picked = null;
            foreach (T candidate in ties)
            {
                if (!nameMatches(candidate))
                {
                    continue;
                }

                if (picked != null)
                {
                    // The name cannot break a tie it matches on both sides of.
                    picked = null;
                    break;
                }

                picked = candidate;
            }

            return true;
        }

        private static bool NameMatches(XenotypeDef def, string name)
        {
            return !name.NullOrEmpty()
                && (string.Equals(def.label, name, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(def.defName, name, StringComparison.OrdinalIgnoreCase));
        }

        private static bool NameMatches(CustomXenotype custom, string name)
        {
            return !name.NullOrEmpty()
                && string.Equals(custom.name?.Trim(), name, StringComparison.OrdinalIgnoreCase);
        }

        // GeneUtility.PawnIsCustomXenotype's rule, applied to two gene lists: every
        // passOnDirectly gene on either side must appear on the other. Genes with
        // passOnDirectly=false (vanilla: none shipped, but modded ones exist) are
        // ignored by both, exactly as vanilla ignores them.
        public static bool GenesMatch(List<GeneDef> germGenes, List<GeneDef> templateGenes)
        {
            if (germGenes == null || templateGenes == null)
            {
                return false;
            }

            foreach (GeneDef gene in templateGenes)
            {
                if (gene.passOnDirectly && !germGenes.Contains(gene))
                {
                    return false;
                }
            }

            foreach (GeneDef gene in germGenes)
            {
                if (gene.passOnDirectly && !templateGenes.Contains(gene))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
