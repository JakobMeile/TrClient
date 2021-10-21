using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrClient;

namespace TrClient
{
    public class clsTrTranscripts : IEnumerable
    {
        private List<clsTrTranscript> Transcripts;
        public int Count { get => Transcripts.Count; }

        public clsTrPage ParentPage;

        public void Add(clsTrTranscript Transcript)
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

        public clsTrTranscript this[int index]
        {
            get { return Transcripts[index]; }
            set { Transcripts[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Transcripts).GetEnumerator();
        }

        public clsTrTranscript GetTranscriptFromID(string Search)
        {
            var Transcript = Transcripts.Where(t => t.ID == Search).FirstOrDefault();
            return Transcript;
        }

        public clsTrTranscripts()
        {
            Transcripts = new List<clsTrTranscript>();
        }

    }
}
