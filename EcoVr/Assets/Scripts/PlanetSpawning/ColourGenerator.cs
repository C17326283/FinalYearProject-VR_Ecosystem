using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for setting the min and max of the material for terrain elevation colours
public class ColourGenerator
{
    private PlanetSettings settings;
    Texture2D[] textures;//texture for assigning gradient to shadergraph
    private int textureResolution = 50;//How big the texture is, this can be small as its just fro gradients
    
    
    //Called when the planet is updated.
    public void UpdateSettings(PlanetSettings settings)
    {
        this.settings = settings;//set the settings to the ones passed in with the constructor
        
        //Make the blank textures for each gradient
        if (textures == null)
        {
            //Make texture array size of however many materials there are
            textures = new Texture2D[settings.biomeGradients.Length];
            for (int i = 0; i < textures.Length; i++)
            {
                //Make the new texture and put into array
                textures[i] = new Texture2D(textureResolution,1);
            }
        }
    }
    
    //update all the material parametes with the elevation heights so terrain levels are always correct
    public void UpdateHeightInShader(TerrainMinMaxHeights elevationMinMax)
    {
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min,elevationMinMax.Max));

    }

    //Colour the texture based on the gradients
    public void UpdateTextureInShader(GameObject biomeObj,GameObject biomeObj2,GameObject biomeObj3,GameObject biomeObj4)
    {
        //Make a different colour for each pixel of texture
        Color[] colours = new Color[textureResolution];
        //for all the textures in array
        for (int i = 0; i < textures.Length; i++)
        {
            //for all the colours
            for (int j = 0; j < textureResolution; j++)
            {
                //Set the colour to the closest on in the pixel array
                colours[j] = settings.biomeGradients[i].Evaluate(j / (textureResolution - 1f));
                textures[i].SetPixels(colours);
                textures[i].Apply();
                //Apply this new texture to the materials parameter for shader graph
                settings.planetMaterial.SetTexture("_texture",textures[0]);//normal
                settings.planetMaterial.SetTexture("_texture2",textures[1]);//hot
                settings.planetMaterial.SetTexture("_texture3",textures[2]);//cold
                settings.planetMaterial.SetVector("_objPos",biomeObj.transform.position);
                settings.planetMaterial.SetVector("_objPos2",biomeObj2.transform.position);
                settings.planetMaterial.SetVector("_objPos3",biomeObj3.transform.position);
                settings.planetMaterial.SetVector("_objPos4",biomeObj4.transform.position);
            }
        }
    }
}
