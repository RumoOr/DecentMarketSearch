using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Accord.Statistics.Models.Regression.Linear;

public class MLRegRating : IRating
{
    private List<double[]> m_Inputs;

    private List<double> m_Outputs;

    private MultipleLinearRegression m_Regression;

    private int Count { get { return m_Inputs.Count; } }

    public MLRegRating()
    {
        Reset();
    }

    public void AddValues(Parcel parcel)
    {
        if (parcel.Price < 0 ||
            parcel.Price >= UIFilter.PRICE_MAXIMUM)
            return;

        m_Inputs.Add(GetValues(parcel));

        m_Outputs.Add(parcel.Price);
    }

    public int GetRating(Parcel parcel)
    {
        if (Count == 0 ||
            parcel.Price < 0 ||
            parcel.Price >= UIFilter.PRICE_MAXIMUM)
            return (int)parcel.Price;

        return (int)m_Regression.Transform(GetValues(parcel));
    }

    public void Learn()
    {
        if (Count == 0)
            return;

        var ols = new OrdinaryLeastSquares()
        {
            UseIntercept = true
        };

        m_Regression = ols.Learn(m_Inputs.ToArray(), m_Outputs.ToArray());
    }

    public void Reset()
    {
        m_Inputs = new List<double[]>();

        m_Outputs = new List<double>();
    }

    private double[] GetValues(Parcel parcel)
    {
        return new double[] {
            parcel.Distance,
            parcel.DistrictDistance,
            parcel.PlazaDistance,
            parcel.RoadDistance
        };
    }
}
