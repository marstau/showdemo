using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCustomData : MonoBehaviour
{
    private Animator _animator;
    private Renderer[] _renderers;

    private void Awake() {
        // 获得动画
        this._animator = this.gameObject.GetComponentInChildren<Animator>();
        // 查询当前所有的meshrender
        this._renderers = this.gameObject.GetComponentsInChildren<Renderer>();
    }

    public Animator getAnimator() {
        return this._animator;
    }

    public void setPower(float f) {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        foreach (var renderer in this._renderers) 
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_zhenying", f);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    // x,y,z为矢量 w为灯光强度
    public void setLightDir(Vector4 V4) {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        foreach (var renderer in this._renderers) 
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetVector("_LightDir", V4);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    public void setShadowColor(Color color) {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        foreach (var renderer in this._renderers) 
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_ShadowCol", color);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
