// <copyright file="TrBaseLineFilter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Helpers
{
    using System.ComponentModel;

    public class TrBaseLineFilter : INotifyPropertyChanged
    {
        private bool coordinatesPositive = true;

        public bool CoordinatesPositive
        {
            get
            {
                return coordinatesPositive;
            }

            set
            {
                coordinatesPositive = value;
                NotifyPropertyChanged("CoordinatesPositive");
            }
        }

        private bool baseLineStraight = true;

        public bool BaseLineStraight
        {
            get
            {
                return baseLineStraight;
            }

            set
            {
                baseLineStraight = value;
                NotifyPropertyChanged("BaseLineStraight");
            }
        }

        private bool baseLineDirectionOK = true;

        public bool BaseLineDirectionOK
        {
            get
            {
                return baseLineDirectionOK;
            }

            set
            {
                baseLineDirectionOK = value;
                NotifyPropertyChanged("BaseLineDirectionOK");
            }
        }

        // Constructor
        public TrBaseLineFilter()
        {
        }

        public void Reset()
        {
            CoordinatesPositive = true;
            BaseLineStraight = true;
            BaseLineDirectionOK = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
