using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParcelFilter
{
    public bool UsePrice { get; set; }
    public bool UseRoad { get; set; }
    public bool UseDistrict { get; set; }
    public bool UsePlaza { get; set; }
    public bool UseHot { get; set; }

    public int PriceMinValue { get; set; }
    public int PriceMaxValue { get; set; }

    public int RoadMaxDist { get; set; }
    public int DistrictMaxDist { get; set; }
    public int PlazaMaxDist { get; set; }
}
