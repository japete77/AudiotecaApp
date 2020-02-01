using System;
using System.Collections.Generic;
using System.Text;

namespace audioteca.ViewModels
{
    public class AudioBookInformationViewModel : ViewModelBase
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged();
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }

        private string _creator;
        public string Creator
        {
            get { return _creator; }
            set
            {
                _creator = value;
                RaisePropertyChanged();
            }
        }

        private string _date;
        public string Date
        {
            get { return _date; }
            set
            {
                _date = value;
                RaisePropertyChanged();
            }
        }

        private string _format;
        public string Format
        {
            get { return _format; }
            set
            {
                _format = value;
                RaisePropertyChanged();
            }
        }

        private string _identifier;
        public string Identifier
        {
            get { return _identifier; }
            set
            {
                _identifier = value;
                RaisePropertyChanged();
            }
        }

        private string _publisher;
        public string Publisher
        {
            get { return _publisher; }
            set
            {
                _publisher = value;
                RaisePropertyChanged();
            }
        }

        private string _subject;
        public string Subject
        {
            get { return _subject; }
            set
            {
                _subject = value;
                RaisePropertyChanged();
            }
        }

        private string _source;
        public string Source
        {
            get { return _source; }
            set
            {
                _source = value;
                RaisePropertyChanged();
            }
        }

        private string _charset;
        public string Charset
    {
            get { return _charset; }
            set
            {
                _charset = value;
                RaisePropertyChanged();
            }
        }

        private string _generator;
        public string Generator
        {
            get { return _generator; }
            set
            {
                _generator = value;
                RaisePropertyChanged();
            }
        }

        private string _narrator;
        public string Narrator
        {
            get { return _narrator; }
            set
            {
                _narrator = value;
                RaisePropertyChanged();
            }
        }

        private string _producer;
        public string Producer
        {
            get { return _producer; }
            set
            {
                _producer = value;
                RaisePropertyChanged();
            }
        }

        private string _totalTime;
        public string TotalTime
        {
            get { return _totalTime; }
            set
            {
                _totalTime = value;
                RaisePropertyChanged();
            }
        }
    }
}
