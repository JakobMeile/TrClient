// <copyright file="TrTranscripts.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class TrTranscripts : IEnumerable
    {
        private List<TrTranscript> transcripts;

        public int Count { get => transcripts.Count; }

        public TrPage ParentPage;

        public void Add(TrTranscript transcript)
        {
            transcripts.Add(transcript);
            transcript.ParentContainer = this;
            transcript.ParentPage = ParentPage;
        }

        public void Sort()
        {
            transcripts.Sort();
        }

        public void Reverse()
        {
            transcripts.Reverse();
        }

        public void Clear()
        {
            transcripts.Clear();
        }

        public TrTranscript this[int index]
        {
            get { return transcripts[index]; }
            set { transcripts[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)transcripts).GetEnumerator();
        }

        public TrTranscript GetTranscriptFromID(string search)
        {
            var transcript = transcripts.Where(t => t.ID == search).FirstOrDefault();
            return transcript;
        }

        public TrTranscripts()
        {
            transcripts = new List<TrTranscript>();
        }
    }
}
