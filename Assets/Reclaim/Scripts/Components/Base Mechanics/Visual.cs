using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[Serializable]
public class NetworkedByteData
{
    [SerializeField]
    public byte[] Data;

    public NetworkedByteData(byte[] data)
    {
        Data = data;
    }
}


[Serializable]
public class VisualData : EntityComponent
{
    public int RenderLayer;
    public Texture2D MapSprite;
    public Texture2D Portrait;
    public TextureFormat MapTextureFormat;

    [SerializeField]
    private byte[] MapSpriteData;
    [SerializeField]
    private byte[] PortraitSpriteData;

    public Type MonobehaviorType = typeof(Visual);

    public override void Serialzie()
    {
        MapTextureFormat = MapSprite.format;
        MapSpriteData = MapSprite.GetRawTextureData();
    }

    public override void DeSerialzie()
    {
        if (MapSprite == null)
        {
            Texture2D mapTexture = new Texture2D(16, 16, MapTextureFormat, false);
            mapTexture.LoadRawTextureData(MapSpriteData);
            mapTexture.filterMode = FilterMode.Point;
            mapTexture.Apply();

            MapSprite = mapTexture;
        }
        //Texture2D portrait = new Texture2D(16, 16);
        //mapTexture.LoadImage(MapSpriteData);
    }
}

public class Visual : ComponentBehavior<VisualData>
{
    void Start()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        component.DeSerialzie();

        //Will have to redo this later
        spriteRenderer.sortingOrder = component.RenderLayer;
        spriteRenderer.sprite = Sprite.Create(component.MapSprite,
            new Rect(0f, 0f, component.MapSprite.width, component.MapSprite.height),
            new Vector2(0.5f, 0.5f),
            16f);
    }
}
