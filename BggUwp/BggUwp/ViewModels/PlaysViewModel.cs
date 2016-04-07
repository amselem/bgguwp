﻿using BggUwp.Data;
using BggUwp.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace BggUwp.ViewModels
{
    public class PlaysViewModel : ViewModelBase
    {
        Windows.UI.Core.CoreDispatcher dispatcher; 

        public PlaysViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
                LoadPlays();
            }
        }

        private ObservableCollection<PlayDataItem> _PlaysList = new ObservableCollection<PlayDataItem>();
        public ObservableCollection<PlayDataItem> PlaysList
        {
            get
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    return DesignDataService.LoadPlays();
                }
                return _PlaysList;
            }
            set
            {
                Set(ref _PlaysList, value);
            }
        }

        private async void LoadPlays()
        {
            var apiPlays = await DataService.Instance.LoadPlays();

            foreach (var item in apiPlays)
            {
                PlaysList.Add(new PlayDataItem(item));
            }
        }
    }
}
