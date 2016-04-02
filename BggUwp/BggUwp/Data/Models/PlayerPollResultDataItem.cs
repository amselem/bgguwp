using BggApi.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Template10.Mvvm;

namespace BggUwp.Data.Models
{
    public class PlayerPollResultDataItem : BindableBase
    {
        private int _NumberOfPlayers = 0;
        public int NumberOfPlayers
        {
            get { return _NumberOfPlayers; }
            set
            {
                Set(ref _NumberOfPlayers, value);
                OnPropertyChanged("NumberOfPlayersDisplay");
            }
        }

        private bool _NumberOfPlayersIsAndHigher = false;
        public bool NumberOfPlayersIsAndHigher
        {
            get { return _NumberOfPlayersIsAndHigher; }
            set
            {
                Set(ref _NumberOfPlayersIsAndHigher, value);
                OnPropertyChanged("NumberOfPlayersDisplay");
            }
        }

        public string NumberOfPlayersDisplay
        {
            get
            {
                return string.Format("{0}{1}", NumberOfPlayers, NumberOfPlayersIsAndHigher ? "+" : string.Empty);
            }
        }

        private int _Best = 0;
        public int Best
        {
            get { return _Best; }
            set
            {
                Set(ref _Best, value);
                RaiseNumbersChanged();
            }
        }

        private int _Recommended = 0;
        public int Recommended
        {
            get { return _Recommended; }
            set
            {
                Set(ref _Recommended, value);
                RaiseNumbersChanged();
            }
        }

        private int _notRecommended = 0;
        public int NotRecommended
        {
            get { return _notRecommended; }
            set
            {
                Set(ref _notRecommended, value);
                RaiseNumbersChanged();
            }
        }

        public int Total
        {
            get
            {
                return Best + Recommended + NotRecommended;
            }
        }
       
        public int BestPercentage
        {
            get
            {
                if (Total == 0)
                    return 0;
                return (int)Math.Round(((decimal)Best / (decimal)Total) * 100);
            }
        }
        
        public int RecommendedPercentage
        {
            get
            {
                if (Total == 0)
                    return 0;
                return (int)Math.Round(((decimal)Recommended / (decimal)Total) * 100);
            }
        }
       
        public int NotRecommendedPercentage
        {
            get
            {
                if (Total == 0)
                    return 0;
                return (int)Math.Round(((decimal)NotRecommended / (decimal)Total) * 100);
            }
        }

        private void RaiseNumbersChanged()
        {
            OnPropertyChanged("Total");
            OnPropertyChanged("BestPercentage");
            OnPropertyChanged("RecommendedPercentage");
            OnPropertyChanged("NotRecommendedPercentage");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
