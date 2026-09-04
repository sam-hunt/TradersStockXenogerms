using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace XenogermTraderStock
{
    // Which xenotype a xenogerm yields: a preset XenotypeDef, or a player-scenario
    // CustomXenotype template. Neither set (None) means no loaded xenotype claims
    // the germ's gene set - an assembler germ matching nothing, or a tie the name
    // could not break - and vanilla's implant is the whole story. Whether the
    // implant patch rewrites the germline for a source is Inheritable's call, not
    // the kind's: a non-inheritable template resolves like any other, and the
    // patch then leaves vanilla's xenogene implant standing.
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
    //  * A matching template is the source, whatever its inheritable flag, and no
    //    preset is considered after it. A player who went to the trouble of
    //    authoring a preset clone and not picking the preset has expressed a
    //    preference, whatever the reason (an interaction with some other mod,
    //    say), and this mod should not overrule it.
    //
    //  * For a NON-inheritable template the implant patch has nothing to rewrite:
    //    vanilla's xenogene implant is exactly what PawnIsCustomXenotype matches
    //    such a template against, so the pawn is the template with no help. The
    //    patch only normalises the pawn's name and icon to the template's, the
    //    same rule a preset match gets.
    //
    //  * For an INHERITABLE template vanilla cannot deliver the template at all:
    //    PawnIsCustomXenotype matches it against ENDOgenes, which a xenogene
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
        // several the one whose name equals xenotypeName (case-insensitive); a tie
        // the name cannot break is None, never a preset - the templates claimed the
        // genes. Only when no template matches: the one preset whose genes match,
        // ties broken against label / defName the same way, otherwise None.
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

            if (customTemplates != null
                && TryPickMatch(customTemplates, germGenes, c => c?.genes, c => NameMatches(c, name),
                    out CustomXenotype template))
            {
                // The player's own xenotypes claimed the genes; a tie the name did
                // not break is vanilla's, not a preset's.
                return template != null ? XenogermSource.Of(template) : XenogermSource.None;
            }

            if (candidates == null)
            {
                return XenogermSource.None;
            }

            TryPickMatch(candidates, germGenes, d => d?.genes, d => NameMatches(d, name), out XenotypeDef preset);
            return preset != null ? XenogermSource.Of(preset) : XenogermSource.None;
        }

        // Shared match-then-tie-break over either candidate kind. Returns whether
        // ANY candidate's genes matched (so a tie among templates can stop the
        // preset pass); `picked` is the single match, or the single name-matching
        // candidate among several, else null.
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
