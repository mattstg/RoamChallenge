using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.CodeDom;


//If your looking for that class with all the enums in it, thats called EnumReferences.cs

//This uses lazy initialization, it will be faster and use less garbage after using it once.

public static class EnumHelper 
{
    
    private static readonly Dictionary<System.Type, System.Enum[]> bakedEnumDict = new Dictionary<System.Type, System.Enum[]>();
    private static readonly Dictionary<Type, string[]> enumNamesDict = new Dictionary<Type, string[]>();
    //private static readonly Dictionary<Enum, string> enumToStringDict = new Dictionary<Enum, string>(); //Dict of enum value to string

    //Lol the very first time it runs it's like spitting in the garbage mans face.
    public static string[] GetNames(this Type enumType)
    {
        try
        {
            if (!enumNamesDict.ContainsKey(enumType))
            {
                AddNewEnumToDictSys(enumType);
            }

            return enumNamesDict[enumType];            
        }
        catch
        {
            Debug.LogError("That probably wasn't an enum type, pass something that's a TypeOf(enum), don't pass an instance or something");
            return new string[] { };
        }
    }

    

    private static void AddNewEnumToDictSys(Type enumType)
    {
        if (enumNamesDict.ContainsKey(enumType))
            return;
        
        System.Enum[] enumsArr = System.Enum.GetValues(enumType).Cast<System.Enum>().ToArray<System.Enum>();

        if (!bakedEnumDict.ContainsKey(enumType))
            bakedEnumDict.Add(enumType, enumsArr);

        enumNamesDict.Add(enumType, bakedEnumDict[enumType].CollectionToStringArray());

        Dictionary<Enum, string> subDict = new Dictionary<Enum, string>();
        try
        {
            foreach (System.Enum e in enumsArr)
                subDict.Add(e, e.ToString());
            TestDict.Add(enumType, subDict);
        }
        catch(System.Exception e)
        {
            Debug.LogError("EnumHelper failed to add enum to dict" + e.Message);
        }
    }


    //So I actually have to create garbage, I cannot store a generic dictionary that holds every possible type of enum, I need to use the shared type: Enum. So then I need to cast it when returning it, that ends up creating garbage.
    //Then Ienumerable clones, ouch, I could only return ints, but that'd force the other side to use an int array or have to translate it.. could do that "out" design pattern haha
    public static T[] GetArrOfEnums<T>() where T : struct, System.IConvertible
    {
        //Not sure if micro opt, but bakes the enums in a dict first time retrieval, since we retreieve some of them alot of times
        if (!bakedEnumDict.ContainsKey(typeof(T)))
            bakedEnumDict.Add(typeof(T), System.Enum.GetValues(typeof(T)).Cast<System.Enum>().ToArray<System.Enum>());

        return bakedEnumDict[typeof(T)].Cast<T>().ToArray<T>();
    }

    public static string ToName(this Enum enumValue)
    {
        if (TestDict.TryGetValue(enumValue.GetType(), out var dict))
            if (dict.TryGetValue(enumValue, out var name))
                return name;

        //Else it failed, add the dict
        AddNewEnumToDictSys(enumValue.GetType());
        if (TestDict.TryGetValue(enumValue.GetType(), out var dict2)) //and try again
            if (dict2.TryGetValue(enumValue, out var name))
                return name;

        //If it still failed, something def wrong
        Debug.LogError("ToName for enum failed" + enumValue.ToString());
        return enumValue.ToString();
    }


    static Dictionary<Type, Dictionary<Enum,string>> TestDict = new Dictionary<Type, Dictionary<Enum, string>>();

    public static void PreloadCommonEnums()
    {
        
        List<Type> commonTypes = new List<Type>()
        {
            /*typeof(CharacterNew),
            typeof(Character),
            typeof(LanguageCode),
            typeof(ParticleType),
            typeof(AIPB_Base.SpecialItem),
            typeof(AIPB_General.BehaviourMode),
            typeof(AIPB_General.TargetingMode),
            typeof(EnemyPool.EnemyTypes),
            typeof(FreeLevelObjectType),
            typeof(FreeLevelObjectClusterParentType),
            typeof(Gas.EffectType),
            typeof(ComputeShaderCache.ShaderName),
            typeof(GameMode),
            typeof(BodyPart),
            typeof(GameplayInputVarType),
            typeof(IE_FullChargeFlash.FlashColor),
            typeof(ProjectileVarLoader.ProjScript),
            typeof(IItemStateEnum),
            typeof(AIBrainType),
            typeof(ItemPrefabNames),
            typeof(RockType),
            typeof(TrapType),
            typeof(NotificationEvent),
            typeof(HitByFireSource),
            typeof(PlayerAbility.AnimationType),
            typeof(BodyPart),
            typeof(BaseMaterial.MaterialType),
            typeof(LocalTextDictionary),
            typeof(PlayerColor),
            typeof(RoundDifficulty)*/
        };

        foreach (Type t in commonTypes)
        {
            AddNewEnumToDictSys(t);
        }
    }


}
