
using UnityEngine;
using System.Collections;

/// <summary>
/// Sortable Range. Min is always smaller or equal to Max
/// </summary>
[System.Serializable]
public struct Range
{
    public float min;
    public float max;

    [LitJson.JsonIgnore]
    public float current
    {
        get
        {
            return this._current;
        }
        set
        {
            if (value != this._current)
            {
                this._current = Clamp (value);
            }
        }
    }
    [LitJson.JsonIgnore]
    private float _current;

    public Range (Vector2 range) : this (Mathf.Min (range[0], range[1]), Mathf.Max (range[0], range[1]))
    {

    }

    public Range (float min, float max) : this (min, max, min)
    {

    }

    public Range (float min, float max, float current)
    {
        this._current = 0;
        this.min = min;
        this.max = max;

        Range.Sort (this);

        this.current = this.min;
    }

    public Range Clone ()
    {
        return new Range (this.min, this.max, this._current);
    }

    #region INSTANCE METHODS
    [LitJson.JsonIgnore]
    public bool IsEmpty
    {
        get
        {
            return min == 0 && max == 0;
        }
    }
    [LitJson.JsonIgnore]
    public bool IsZeroLength
    {
        get { return min == max; }
    }
    [LitJson.JsonIgnore]
    public float Avarege
    {
        get
        {
            return (this.min + this.max) / 2;
        }
    }
    [LitJson.JsonIgnore]
    public float MinToMaxRatio
    {
        get
        {
            if (max != 0)
                return min / max;
            return this.min / float.Epsilon;
        }
    }
    [LitJson.JsonIgnore]
    public float MaxToMinRatio
    {
        get
        {
            if (min != 0)
                return max / min;
            return max / float.Epsilon;
        }
    }

    //Returns current value to max value ratio
    [LitJson.JsonIgnore]
    public float CurrentToMaxRatio
    {
        get
        {
            if (max != 0)
                return this.current / this.max;
            return this.current / float.Epsilon;
        }
    }

    // Returns current distance from minimum
    [LitJson.JsonIgnore]
    public float CurrentOffset
    {
        get
        {
            return this._current - this.min;
        }
    }

    public float GetRandom ()
    {
        return UnityEngine.Random.Range (this.min, this.max);
    }
    [LitJson.JsonIgnore]
    public float Random
    {
        get { return UnityEngine.Random.Range (this.min, this.max); }
    }
    [LitJson.JsonIgnore]
    public int IntRandom
    {
        get { return UnityEngine.Random.Range ((int)this.min, (int)this.max); }
    }
    [LitJson.JsonIgnore]
    public float length
    {
        get
        {
            return this.max - this.min;
        }
    }

    public float Percentage (float value)
    {
        if (!this.IsZeroLength)
            return Mathf.Clamp01 ((value - this.min) / (this.max - this.min));
        return 0;
    }
    [LitJson.JsonIgnore]
    public float CurrentPercentage
    {

        get
        {
            if (!this.IsZeroLength)
                return (this._current - this.min) / (this.max - this.min);
            return 0;
        }
    }

    public float MappedPoint (float normalizedInput)
    {
        return (max - min) * normalizedInput + min;
    }

    public bool Contains (float point)
    {
        return (point >= min && point <= max);
    }

    public void Negate ()
    {
        float _cache = 0;
        _cache = -this.min;
        this.min = -this.max;
        this.max = _cache;
    }

    public void Offset (float offset)
    {
        this.min += offset;
        this.max += offset;
    }

    public void Spread (float value)
    {
        value *= 0.5f;
        this.min -= value;
        this.max += value;
    }

    public void Scale (float scale)
    {
        this.min *= scale;
        this.max *= scale;
    }

    public float Clamp (float value)
    {
        if (value < this.min)
            return min;
        if (value > this.max)
            return max;
        return value;
    }

    #endregion

    #region STATIC METHODS

    public static void Sort (Range range)
    {
        float _cache = range.min;
        if (range.min > range.max)
        {
            range.min = range.max;
            range.max = _cache;
        }
    }
    [LitJson.JsonIgnore]
    public static Range clamp01
    {
        get { return new Range (0f, 1f); }
    }
    [LitJson.JsonIgnore]
    public static Range clamp100
    {
        get { return new Range (0, 100); }
    }

    #endregion

    #region OVERRIDES

    public override string ToString ()
    {
        return string.Format ("[Min - {0:##.###} , Max - {1:##.###}]", this.min, this.max);
    }

    #endregion
    [LitJson.JsonIgnore]
    public float this[int index]
    {
        get
        {
            if (index == 0)
                return this.min;
            else if (index == 1)
                return this.max;
            else if (index == 2)
                return this.current;
            else
                throw new System.ArgumentOutOfRangeException ("index");
        }
        set
        {
            if (index == 0)
                this.min = value;
            else if (index == 1)
                this.max = value;
            else if (index == 2)
                this.current = value;
            else
                throw new System.ArgumentOutOfRangeException ("index");
        }
    }
}
