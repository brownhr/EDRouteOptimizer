using Caliburn.Micro;
using EDRouteOptimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfEDRO.Models;

namespace wpfEDRO.ViewModels
{
    public class ShellViewModel : Screen
    {
        private string _subSector = "[Testing]";
        private string _boxelCode;

        private BindableCollection<SubSectorModel> _subSectors = new BindableCollection<SubSectorModel>();
        private SubSectorModel _selectedSubSector;

        public ShellViewModel()
        {
            List<EDSubsector> children = EDSubsector.GetSubsector("Graea Hypue RT-Y d2").GetChildSubsectors('b');
        }

        public string SubSector
        {
            get
            {
                return _subSector;
            }
            set
            {
                _subSector = value;
                NotifyOfPropertyChange(() => SubSector);
                NotifyOfPropertyChange(() => FullName);
            }
        }
        public string BoxelCode
        {
            get
            {
                return _boxelCode;
            }
            set
            {
                _boxelCode = value;
                NotifyOfPropertyChange(() => BoxelCode);
                NotifyOfPropertyChange(() => FullName);

            }
        }
        public string FullName
        {
            get { return $"{SubSector} {BoxelCode}"; }

        }

        public BindableCollection<SubSectorModel> SubSectors
        {
            get { return _subSectors; }
            set { _subSectors = value; }
        }

        public SubSectorModel SelectedSubSector
        {
            get { return _selectedSubSector; }
            set
            {
                _selectedSubSector = value;
                NotifyOfPropertyChange(() => SelectedSubSector);
            }
        }




    }
}
