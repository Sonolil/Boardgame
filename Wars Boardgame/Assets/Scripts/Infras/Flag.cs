using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flag : Infra {
    void Start()
    {
        Init();
        StartCoroutine("Wake");
    }

    protected override void Init()
    {
        base.Init();

    }

}
