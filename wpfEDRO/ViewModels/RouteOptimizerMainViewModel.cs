using Caliburn.Micro;
using EDRouteOptimizer;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfEDRO.ViewModels
{
    public class RouteOptimizerMainViewModel : Screen
    {
        private string _filename;
        // TODO: Implement ViewModel

        private EDRoute _route;
        private BindableCollection<EDSystem> _waypoints = new BindableCollection<EDSystem>();
        private BindableCollection<FSSAllBodiesFoundEvent> _mappedSystems
            = new BindableCollection<FSSAllBodiesFoundEvent>();


        private string _systemName;

        private EDSystem _currentSystem;

        private DateTime _dateTimeCutoff = DateTime.ParseExact("2023-03-01",
                                                               format: "yyyy-MM-dd",
                                                               CultureInfo.InvariantCulture);

        public string SystemName
        {
            get { return _systemName; }
            set
            {
                _systemName = value;
                NotifyOfPropertyChange(() => SystemName);
            }
        }

        public DateTime DateTimeCutoff
        {
            get { return _dateTimeCutoff; }
            set
            {
                _dateTimeCutoff = value;
                NotifyOfPropertyChange(() => DateTimeCutoff);
            }
        }


        public EDSystem CurrentSystem
        {
            get { return _currentSystem; }
            set
            {
                _currentSystem = value;
                NotifyOfPropertyChange(() => CurrentSystem);
            }
        }

        public BindableCollection<EDSystem> Waypoints
        {
            get { return _waypoints; }
            set { _waypoints = value; }
        }

        public BindableCollection<FSSAllBodiesFoundEvent> MappedSystems
        {
            get { return _mappedSystems; }
            set { _mappedSystems = value; }
        }

        public RouteOptimizerMainViewModel()
        {
            Filename = "";
            Route = new EDRoute();
        }

        public string Filename
        {
            get { return _filename; }
            set
            {
                _filename = value;
                NotifyOfPropertyChange(() => Filename);
            }
        }

        public EDRoute Route
        {
            get { return _route; }
            set
            {
                _route = value;
                NotifyOfPropertyChange(() => Route);
            }
        }


        public void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = @"Route Files|*.route;*.json";
            if (ofd.ShowDialog() == true)
            {
                Filename = ofd.FileName;
            }
        }

        public void ParseRoute()
        {
            EDEventParser.SetCutoff(DateTimeCutoff);
            EDEventParser.ParseAllJournals();
            EDEventParser.ParseJournalEvents();
            MappedSystems
                = new BindableCollection<FSSAllBodiesFoundEvent>(EDEventParser.FSSAllBodiesFoundEvents);

            CurrentSystem = EDEventParser.CurrentSystem;
            Route = EDRoute.ParseJson(Filename);
            Waypoints = new BindableCollection<EDSystem>(Route.RouteWaypoints);

        }

        public bool CanParseRoute(string fileName)
        {
            return !string.IsNullOrWhiteSpace(fileName);
        }
    }
}
