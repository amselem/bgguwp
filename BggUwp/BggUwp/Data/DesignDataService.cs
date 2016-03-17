using BggUwp.Data.Models;
using BggUwp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BggUwp.Views
{
    public class DesignDataService
    {
        public DesignDataService()
        {
            LoadHotItemsList();
        }

        private HotItemsViewModel _HotItemsVM = new HotItemsViewModel();
        public HotItemsViewModel HotItemsVM
        {
            get
            {
                return _HotItemsVM;
            }
        }

        public void LoadHotItemsList()
        {
            HotDataItem item = new HotDataItem();
            item.Name = "Design time HotItems item";
            item.YearPublished = 1999;
            HotItemsVM.HotItemsList.Add(item);

            HotDataItem item2 = new HotDataItem();
            item2.Name = "Design time HotItems item - second item";
            item2.YearPublished = 2999;
            HotItemsVM.HotItemsList.Add(item2);

            HotDataItem item3 = new HotDataItem();
            item3.Name = "Design time HotItems item - third item";
            item3.YearPublished = 3999;
            HotItemsVM.HotItemsList.Add(item3);
        }
    }
}
