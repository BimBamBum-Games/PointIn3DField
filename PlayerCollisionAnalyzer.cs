using System.Text;
using UnityEngine;

public class PlayerCollisionAnalyzer : MonoBehaviour
{
    //Add this gameobject collider and rigidbody set rigidbody isKinematic fields to true.
    //Add the collider components to other gameobjects and set their isTrigger fields to true and also set tags on inspector panel.
    //Prefab Parent State : DoubleWay -> DoubleWayBasement, DoubleWayDetectionField, DoubleWayBasement -> DoubleWayLeft, DoubleWayRight
    //In addition, DoubleWayDetectionField Can Be Pull Into A Different Layer or Can Be UnTrigger.
    //Thus Player Wont Collide With It. To Do This, You Have To Use Collision Matrix

    public Transform pointerOnBounds_tr;
    private ClosestPointOnTheBounds _clsp;
    private Collider _otherPivot_cl;

    private void Awake()
    {
        _clsp = ClosestPointOnTheBounds.GetDefaultCPOB(transform, pointerOnBounds_tr);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DoubleWayLeft"))
        {
            CheckIfPointOnBounds(other);
        }
        if (other.CompareTag("DoubleWayRight"))
        {
            CheckIfPointOnBounds(other);
        }
    }

    public void CheckIfPointOnBounds(Collider other)
    {
        Debug.Log(other.tag);
        Vector3 pnt = _clsp.Calculate(other).GetPoint();
        _otherPivot_cl = other.transform.parent.parent.GetComponent<DoubleWay>().availableDetectionField_cl;
        bool decision = ClosestPointOnTheBounds.GetResultPointInOther(_otherPivot_cl, pnt);
        if (decision) Debug.LogWarning("This Point Is On The Specific Collider Bounds!");
        else Debug.LogWarning("This Point Is Not On The Specific Collider Bounds!");
    }
}


public class ClosestPointOnTheBounds
{
    private Vector3 _pointClosestOnBounds_vc;
    private Transform _gameObject_tr;
    private Transform _pointer_tr;
    private bool _isDebugging;
    private string _broadCast;

    //Hide Constructor Bloch
    private ClosestPointOnTheBounds(Transform gameObject_tr, Transform pointer_tr, bool isDebugging = false)
    {
        //The Consturctor will be used for FactoryMethod with Bloch
        _gameObject_tr = gameObject_tr;
        _pointer_tr = pointer_tr;
        _isDebugging = isDebugging;
    }

    //Bloch Factory Method
    public static ClosestPointOnTheBounds GetDefaultCPOB(Transform gameObject_tr, Transform pointer_tr)
    {      
        //Give me an instance of this class
        return new ClosestPointOnTheBounds(gameObject_tr, pointer_tr);
    }

    public static ClosestPointOnTheBounds GetDebuggedCPOB(Transform gameObject_tr, Transform pointer_tr, bool isDebugging)
    {
        //Give me an instance of this class
        return new ClosestPointOnTheBounds(gameObject_tr, pointer_tr, isDebugging);
    }

    public ClosestPointOnTheBounds Calculate(Collider other)
    {
        GetClosestPointOnBounds(other, _gameObject_tr.position);
        SetPointerOnClosestPointOnBounds(_pointer_tr);
        _broadCast = "Other Collider Tag >>>>> " + other.tag;
        if(_isDebugging) BroadcastDebug(_broadCast);
        return this;
    }

    public Vector3 GetPoint()
    {
        return _pointClosestOnBounds_vc;
    }

    private void GetClosestPointOnBounds(Collider other, Vector3 givenPos)
    {
        //Finds the closest point on surface bounds.
        _pointClosestOnBounds_vc = other.ClosestPointOnBounds(givenPos);
        _broadCast =  "ClosestPointOnBounds is >>>>> " + _pointClosestOnBounds_vc;
        if(_isDebugging) BroadcastDebug(_broadCast);
    }
    private void SetPointerOnClosestPointOnBounds(Transform pointer)
    {
        //On Trigger moment place the pointer on the point which is over the bounds of the collider. (Dunya Koordinati Seklinde Deger Atandi.)
        pointer.position = _pointClosestOnBounds_vc;
    }

    public static bool GetResultPointInOther(Collider col, Vector3 desiredVector)
    {
        //Th Closest Point that has found is on the collider bounds.
        bool checkResult = col.bounds.Contains(desiredVector);
        return checkResult;
    }

    public void BroadcastDebug(string str)
    {
        //Debugger Method
        if (_isDebugging)
        {
            Debug.LogWarning(str);
        }
    }
}

