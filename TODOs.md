# TODOs

- **Key the hardcoded English in the pricing StatParts.** `StatPart_XenogermValue.ExplanationPart` ("Preset xenotype (...)", "Scenario xenotype (...)", "Genetic metabolism", "Genetic complexity", "Archite genes") and `StatPart_XenogermSellFactor.ExplanationPart` ("Archite genes (n): +x%") build the info-card breakdown from C# string literals, the only player-facing text outside `XTS_UI.xml`. Move them to `XTS_` keys (with `{0}` placeholders), then run the `translate` skill for the nine languages and `check-translations.py`.
