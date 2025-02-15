using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URPMain : MonoBehaviour
{
    private Light _light;
    private float _orientation;

    void Awake()
    {
        _light = GameObject.Find("Directional Light").GetComponent<Light>();
        _orientation = (int)_light.transform.localEulerAngles.x;
    }

    void FixedUpdate()
    {
        var eular = _light.transform.localEulerAngles;
        _orientation += Time.deltaTime;
        if (_orientation >= 360)
        {
            _orientation = 0;
        }
        eular.x = _orientation++;
        _light.transform.localEulerAngles = eular;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
