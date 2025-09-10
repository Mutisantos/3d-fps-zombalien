using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Script para mover o parar plataformas móviles
 * Esteban.Hernandez
 */
public class PlatformSwitch : MonoBehaviour
{

    public MovingPlatform ControlledPlatform;
    public bool ActiveSwitch;
    public Color DisabledColor;
    public Color EnabledColor;
    private MeshRenderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }


    public void Hit()
    {
        ActiveSwitch = !ActiveSwitch;
        ControlledPlatform.EnablePlatform(ActiveSwitch);
        if (!ActiveSwitch)
        {
            _renderer.material.color = DisabledColor;
        }
        else
        {
            _renderer.material.color = EnabledColor;
        }
    }
}
