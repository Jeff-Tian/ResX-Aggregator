// Guids.cs
// MUST match guids.h
using System;

namespace zizhujycom.ResX_Aggregator
{
    static class GuidList
    {
        public const string guidResX_AggregatorPkgString = "74ffe834-3a9f-4aa8-a53e-e3b0348d1925";
        public const string guidResX_AggregatorCmdSetString = "5f87a242-5991-4af7-bd38-cc9374dfd0d9";
        public const string guidResX_AggregatorEditorFactoryString = "e0c64bd3-1eb9-4243-bbc0-6e8cd846f292";

        public static readonly Guid guidResX_AggregatorCmdSet = new Guid(guidResX_AggregatorCmdSetString);
        public static readonly Guid guidResX_AggregatorEditorFactory = new Guid(guidResX_AggregatorEditorFactoryString);
    };
}