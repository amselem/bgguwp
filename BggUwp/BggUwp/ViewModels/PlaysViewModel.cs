using BggUwp.Data;
using BggUwp.Data.Models;
using BggUwp.Messaging;
using GalaSoft.MvvmLight.Messaging;
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
                Messenger.Default.Register<RefreshDataMessage>(this, RefreshData);
            }
            LoadPlays();
        }

        private ObservableCollection<GroupInfoList> _PlaysList = new ObservableCollection<GroupInfoList>();
        public ObservableCollection<GroupInfoList> PlaysList
        {
            get
            {

                return _PlaysList;
            }
            set
            {
                Set(ref _PlaysList, value);
            }
        }

        public async void LoadPlays()
        {
            var plays = new ObservableCollection<PlayDataItem>();
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                plays = DesignDataService.LoadPlays();
            }
            else
            {
                var apiPlays = await DataService.Instance.LoadPlays();

                foreach (var item in apiPlays)
                {
                    plays.Add(new PlayDataItem(item));
                }
            }

            ObservableCollection<GroupInfoList> groups = new ObservableCollection<GroupInfoList>();

            var query = from item in plays
                        group item by item.PlayDate into grp
                        orderby grp.Key descending
                        select new { GroupName = grp.Key, Items = grp };

            foreach (var grp in query)
            {
                GroupInfoList info = new GroupInfoList();
                info.Key = grp.GroupName;
                foreach (var item in grp.Items)
                {
                    info.Add(item);
                }
                groups.Add(info);
            }

            PlaysList = groups;
        }

        private void RefreshData(RefreshDataMessage msg)
        {
            if (msg.RequestedRefreshScope == RefreshDataMessage.RefreshScope.All ||
                msg.RequestedRefreshScope == RefreshDataMessage.RefreshScope.Plays)
            {
                LoadPlays();
            }
        }

    }
}