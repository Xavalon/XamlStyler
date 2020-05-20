using System;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.Threading;
using JetBrains.Util;

namespace ReSharperPlugin.XamlStyler.dotUltimate.Options
{
    public class StringListViewModel
    {
        private readonly GroupingEvent myEntryChanged;
        private readonly Lifetime myLifetime;
        [NotNull] private readonly Property<string> mySource;

        public StringListViewModel(
            [NotNull] Lifetime lifetime,
            [NotNull] Property<string> source)
        {
            myLifetime = lifetime;
            mySource = source;
            myEntryChanged = new GroupingEventHost(lifetime, false).CreateEvent(lifetime,
                "StringListViewModel.EntryChangedGrouped",
                TimeSpan.FromMilliseconds(100),
                OnEntryChanged);

            var entries = mySource.Value.SplitByNewLine()
                .Where(entry => !entry.IsEmpty())
                .Select(entry => new StringListEntry(lifetime, myEntryChanged.Incoming, entry.Trim()))
                .ToList();

            Entries = new ListEvents<StringListEntry>(lifetime, "StringListViewModel.Entries", entries, false);
            SelectedEntry = new Property<StringListEntry>(lifetime, "StringListViewModel.SelectedEntry");
        }

        public ListEvents<StringListEntry> Entries { get; }
        public IProperty<StringListEntry> SelectedEntry { get; }

        public StringListEntry CreateNewEntry()
        {
            return new StringListEntry(myLifetime, myEntryChanged.Incoming, string.Empty);
        }

        public void OnEntryRemoved()
        {
            myEntryChanged.Incoming.Fire();
        }

        public void OnEntryMoved()
        {
            myEntryChanged.Incoming.Fire();
        }

        private void OnEntryChanged()
        {
            var entries = Entries
                .Select(entry => entry.Value.Value.Trim())
                .Where(entry => !entry.IsEmpty())
                .ToArray();

            mySource.Value = string.Join("\n", entries);
        }
        
        public class StringListEntry
        {
            public readonly IProperty<string> Value;

            public StringListEntry(
                [NotNull] Lifetime lifetime,
                [NotNull] ISimpleSignal entryChanged,
                string value)
            {
                Value = new Property<string>(lifetime, "StringListEntry.Value", value);
                Value.Change.Advise_NoAcknowledgement(lifetime, entryChanged.Fire);
            }
        }
    }
}