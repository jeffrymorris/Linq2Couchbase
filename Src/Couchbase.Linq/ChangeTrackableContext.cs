using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Couchbase.Linq
{
    internal interface IChangeTrackableContext
    {
        void Track<T>(T document);

        void Untrack<T>(T document);

        void Modified<T>(T document);

        int ModifiedCount { get; }

        int TrackedCount { get; }
    }
}
