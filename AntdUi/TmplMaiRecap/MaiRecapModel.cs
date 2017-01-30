using System;
using System.Collections.Generic;
using System.Linq;

namespace AntdUi.TmplMaiRecap {
    public class MaiRecapModel : Database.BaseModel
    {
        /// <summary>
        /// Ward + " " + Month
        /// </summary>
        public string FileName { get; set; }

        public DateTime Date { get; set; }

        /// <summary>
        /// Format: MMMM yyyy
        /// </summary>
        public string Month { get; set; }

        public string Ward { get; set; }
        public string Nurse { get; set; }
        public RequestStatus Status { get; set; }

        public int TotalBed { get; set; } = 0;
        public int TotalAvailableSupport { get; set; } = 0;

        public int TotalKitchenClean { get; set; } = 0;
        public int TotalNightDiaperChange { get; set; } = 0;

        public IEnumerable<Guest> Guests { get; set; } = new List<Guest>();

        public int TotalFemaleGuest => Guests.Count(_ => _.Gender == 'F');
        public int TotalMaleGuest => Guests.Count(_ => _.Gender == 'M');

        public int TotalBath => Guests.Select(_ => _.TotalBath).Sum();
        public int TotalNail => Guests.Select(_ => _.TotalNail).Sum();
        public int TotalFemaleShave => Guests.Select(_ => _.TotalFemaleShave).Sum();
        public int TotalMaleShave => Guests.Select(_ => _.TotalMaleShave).Sum();
        public int TotalBarber => Guests.Select(_ => _.TotalBarber).Sum();
        public int TotalSupport => Guests.Select(_ => _.TotalSupport).Sum();
        public int TotalBedClean => Guests.Select(_ => _.TotalBedClean).Sum();
        public int TotalNotMobilized => Guests.Select(_ => _.TotalNotMobilized).Sum();
        public int TotalRest => Guests.Select(_ => _.TotalRest).Sum();

        public double AverageBath => TotalBed > 0 ? Math.Round(TotalBath / (double)TotalBed, 2) : 0;
        public double AverageNail => TotalBed > 0 ? Math.Round(TotalNail / (double)TotalBed, 2) : 0;
        public double AverageFemaleShave => TotalFemaleGuest > 0 ? Math.Round(TotalFemaleShave / (double)TotalFemaleGuest, 2) : 0;
        public double AverageMaleShave => TotalMaleGuest > 0 ? Math.Round(TotalMaleShave / (double)TotalMaleGuest, 2) : 0;
        public double AverageBarber => TotalBed > 0 ? Math.Round(TotalBarber / (double)TotalBed, 2) : 0;
        public double AverageSupport => TotalAvailableSupport > 0 ? Math.Round(TotalSupport / (double)TotalAvailableSupport, 2) : 0;
        public double AverageBedClean => TotalBed > 0 ? Math.Round(TotalBedClean / (double)TotalBed, 2) : 0;
        public double AverageNotMobilized => TotalBed > 0 ? Math.Round(TotalNotMobilized / (double)TotalBed, 2) : 0;
        public double AverageRest => TotalBed > 0 ? Math.Round(TotalRest / (double)TotalBed, 2) : 0;
    }

    public class Guest {
        /// <summary>
        /// Questo valore deve essere minore di MaiRecapModel.TotalBed
        /// </summary>
        public int BedIndex { get; set; }
        public char Gender { get; set; }
        public string GuestName { get; set; }

        /// <summary>
        /// Data di "uscita"
        /// </summary>
        public string Exit { get; set; } = "";
        public string Note { get; set; }

        public int TotalBath { get; set; } = 0;
        public int TotalNail { get; set; } = 0;
        public int TotalFemaleShave { get; set; } = 0;
        public int TotalMaleShave { get; set; } = 0;
        public int TotalBarber { get; set; } = 0;
        public int TotalSupport { get; set; } = 0;
        public int TotalBedClean { get; set; } = 0;
        public int TotalNotMobilized { get; set; } = 0;
        public int TotalRest { get; set; } = 0;
    }
}