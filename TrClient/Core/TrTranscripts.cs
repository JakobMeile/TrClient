using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrClient;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;


namespace TrClient.Core
{
    public class TrTranscripts : IEnumerable
    {
        private List<TrTranscript> Transcripts;
        public int Count { get => Transcripts.Count; }

        public TrPage ParentPage;

        public void Add(TrTranscript Transcript)
        {
            Transcripts.Add(Transcript);
            Transcript.ParentContainer = this;
            Transcript.ParentPage = this.ParentPage;
        }

        public void Sort()
        {
            Transcripts.Sort();
        }

        public void Reverse()
        {
            Transcripts.Reverse();
        }

        public void Clear()
        {
            Transcripts.Clear();
        }

        public TrTranscript this[int index]
        {
            get { return Transcripts[index]; }
            set { Transcripts[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Transcripts).GetEnumerator();
        }

        public TrTranscript GetTranscriptFromID(string Search)
        {
            var Transcript = Transcripts.Where(t => t.ID == Search).FirstOrDefault();
            return Transcript;
        }

        public TrTranscripts()
        {
            Transcripts = new List<TrTranscript>();
        }

    }
}
