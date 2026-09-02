using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace XenogermTraderStock
{
    // Vanilla's Widgets.IntRange minus its centred grey "min - max" readout,
    // and minus the line of height that readout needed. Our rows already
    // print the numbers in the label above the slider, so the vanilla one was
    // a second copy of them wedged between the label and its own control.
    //
    // Everything else is lifted from Widgets.IntRange so the thing still feels
    // like every other range slider in the game: the 8px side insets, the 2px
    // rail with the selected span drawn over it, the mirrored 16px handle
    // texture, click-grabs-the-nearer-end, the throttled drag sound. The drag
    // state has to be ours (vanilla's statics are private), which also keeps a
    // drag here from being confused with one on a vanilla slider.
    //
    // The attribute is load-bearing, not decoration: dev mode's
    // StaticConstructorOnStartupUtility.ReportProbablyMissingAttributes warns
    // about ANY static Texture field on a type without it (the smoke gate
    // counts that warning against us), and CallAll runs this cctor after
    // content is loaded, so the texture can be resolved eagerly like
    // vanilla's own Widgets does.
    [StaticConstructorOnStartup]
    public static class CompactIntRange
    {
        // Room for the handles - 16px tall, centred on a rail sitting 8px up
        // from the bottom edge, exactly where vanilla puts them - plus a
        // little headroom. Vanilla's 32 was this plus the label line.
        public const float Height = 18f;

        private const float SideInset = 8f;
        private const float HandleSize = 16f;

        // Vanilla's RangeControlTextColor, which it also uses for the rail.
        private static readonly Color RailColor = new Color(0.6f, 0.6f, 0.6f);

        private enum DragEnd
        {
            None,
            Min,
            Max,
        }

        private static int draggingId;
        private static DragEnd curDragEnd;
        private static float lastDragSoundTime = -1f;

        private static readonly Texture2D HandleTex = ContentFinder<Texture2D>.Get("UI/Widgets/RangeSlider");

        public static void Draw(Listing_Standard listing, ref IntRange range, int min, int max)
        {
            Rect rect = listing.GetRect(Height);
            // Same id scheme as Listing_Standard.IntRange: the listing's
            // running height is unique per row and stable across frames.
            Draw(rect, (int)listing.CurHeight, ref range, min, max);
            listing.Gap(listing.verticalSpacing);
        }

        public static void Draw(Rect rect, int id, ref IntRange range, int min, int max)
        {
            Rect inner = rect;
            inner.xMin += SideInset;
            inner.xMax -= SideInset;

            Color prevColor = GUI.color;
            Rect rail = new Rect(inner.x, inner.yMax - 9f, inner.width, 2f);
            GUI.color = RailColor;
            GUI.DrawTexture(rail, BaseContent.WhiteTex);
            GUI.color = Color.white;

            float minX = inner.x + (inner.width * (range.min - min) / (max - min));
            float maxX = inner.x + (inner.width * (range.max - min) / (max - min));
            GUI.DrawTexture(new Rect(minX, inner.yMax - 10f, maxX - minX, 4f), BaseContent.WhiteTex);

            // One texture for both ends: the max handle is drawn with a
            // negative width, which mirrors it, so the pair faces inwards.
            GUI.DrawTexture(new Rect(minX - HandleSize, rail.center.y - 8f, HandleSize, HandleSize), HandleTex);
            GUI.DrawTexture(new Rect(maxX + HandleSize, rail.center.y - 8f, -HandleSize, HandleSize), HandleTex);
            GUI.color = prevColor;

            if (curDragEnd != DragEnd.None
                && (Event.current.type == EventType.MouseUp || Event.current.rawType == EventType.MouseDown))
            {
                draggingId = 0;
                curDragEnd = DragEnd.None;
                SoundDefOf.DragSlider.PlayOneShotOnCamera();
            }

            bool justGrabbed = false;
            if (Mouse.IsOver(rect) || draggingId == id)
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && id != draggingId)
                {
                    draggingId = id;
                    // Outside the pair, the near end; between them, whichever
                    // end the click landed closer to.
                    float x = Event.current.mousePosition.x;
                    curDragEnd = x < minX ? DragEnd.Min
                        : x > maxX + HandleSize ? DragEnd.Max
                        : Mathf.Abs(x - minX) < Mathf.Abs(x - maxX) ? DragEnd.Min
                        : DragEnd.Max;
                    justGrabbed = true;
                    Event.current.Use();
                    SoundDefOf.DragSlider.PlayOneShotOnCamera();
                }

                if (justGrabbed || (curDragEnd != DragEnd.None && UnityGUIBugsFixer.MouseDrag()))
                {
                    int value = Mathf.RoundToInt(Mathf.Clamp(
                        ((Event.current.mousePosition.x - inner.x) / inner.width * (max - min)) + min, min, max));
                    // The dragged end pushes the other one along rather than
                    // crossing it, so the range never inverts.
                    if (curDragEnd == DragEnd.Min && value != range.min)
                    {
                        range.min = value;
                        if (range.max < range.min)
                        {
                            range.max = range.min;
                        }
                        PlayDragSound();
                    }
                    else if (curDragEnd == DragEnd.Max && value != range.max)
                    {
                        range.max = value;
                        if (range.min > range.max)
                        {
                            range.min = range.max;
                        }
                        PlayDragSound();
                    }

                    if (Event.current.type == EventType.MouseDrag)
                    {
                        Event.current.Use();
                    }
                }
            }
        }

        // Vanilla's CheckPlayDragSliderSound: at most one tick per 75ms, so a
        // fast drag across the bar doesn't machine-gun the sound.
        private static void PlayDragSound()
        {
            if (Time.realtimeSinceStartup > lastDragSoundTime + 0.075f)
            {
                SoundDefOf.DragSlider.PlayOneShotOnCamera();
                lastDragSoundTime = Time.realtimeSinceStartup;
            }
        }
    }
}
