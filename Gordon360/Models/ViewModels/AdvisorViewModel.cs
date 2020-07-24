using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class AdvisorViewModel
    {
        public AdvisorViewModel(string fname, string lname, string adname)
        {
            Firstname = fname;
            Lastname = lname;
            ADUserName = adname;
        }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ADUserName { get; set; }
    }

    // Collection of Advisor objects. This class
    // implements IEnumerable so that it can be used
    // with ForEach syntax.
    public class AdvisorIDEnumerable : IEnumerable
    {
        private AdvisorViewModel[] _advisorID;

        public AdvisorIDEnumerable(AdvisorViewModel[] advisorsID)
        {
            _advisorID = new AdvisorViewModel[advisorsID.Length];
            for (int i = 0; i < advisorsID.Length; i++)
            {
                //Make a object array equal object advisor array     
                _advisorID[i] = advisorsID[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public AdvisorsIDEnumerator GetEnumerator()
        {
            return new AdvisorsIDEnumerator(_advisorID);
        }
    }
    // When you implement IEnumerable(T), you must also implement IEnumerator(T),
    // which will walk through the contents of the file one line at a time.
    // Implementing IEnumerator(T) requires that you implement IEnumerator and IDisposable.
    public class AdvisorsIDEnumerator : IEnumerator
    {
        private AdvisorViewModel[] _aID;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public AdvisorsIDEnumerator(AdvisorViewModel[] advisorsID)
        {
            _aID = advisorsID;
        }

        // Implement the IEnumerator(T).Current publicly, but implement
        // IEnumerator.Current, which is also required, privately.
        public AdvisorViewModel Current
        {

            get
            {
                try
                {
                    return _aID[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        // Implement MoveNext and Reset, which are required by IEnumerator.
        public bool MoveNext()
        {
            position++;
            return (position < _aID.Length);
        }

        public void Reset()
        {
            position = -1;
        }
    }
}