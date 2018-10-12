using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRating
{
    void AddValues(Parcel parcel);

    void Learn();

    int GetRating(Parcel parcel);

    void Reset(); 
}
