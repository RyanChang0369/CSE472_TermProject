using System;
using Newtonsoft.Json;

#region Example
// BEGIN EXAMPLE:

// The following example demonstrates a dual-implementation of the ISaveLoadable
// interface.

// public class MainClass : ISaveLoadable<MainClass.Data1>,
//     ISaveLoadable<MainClass.Data2>
// {
//     public struct Data1 : ISaveState
//     {
//         // variables ...

//         public object LoadObject()
//         {
//             MainClass main = // Create instance of main class ...

//             // Load values.
//             main.LoadState(this);

//             return main;
//         }
//     }

//     public struct Data2 : ISaveState
//     {
//         // variables ...

//         public object LoadObject()
//         {
//             MainClass main = // Create instance of main class ...

//             // Load values.
//             main.LoadState(this);

//             return main;
//         }
//     }


//     Data1 ISaveLoadable<Data1>.SaveState()
//     {
//         return new Data1()
//         {
//             // instantiate with variables from MainClass ...
//         };
//     }

//     public void LoadState(Data1 data)
//     {
//         // Apply values from data here ...
//     }

//     Data2 ISaveLoadable<Data2>.SaveState()
//     {
//         return new Data2()
//         {
//             // instantiate with variables from MainClass ...
//         };
//     }

//     public void LoadState(Data2 data)
//     {
//         throw new System.NotImplementedException();
//     }
// }
// END EXAMPLE
#endregion

/// <summary>
/// Note: I assume that you have basic knowledge of JSON serialization. If not,
/// see <see href="https://www.newtonsoft.com/json/help/html/Introduction.htm"/>
/// for an introduction on JSON serialization.
///
/// <br/>
///
/// An interface that incorporates saving and loading methods, allowing for the
/// customization of serialization. We could just serialize the class directly,
/// but that would make it difficult to work with when we start to load the
/// object again.
///
/// <br/>
///
/// To make a class implement ISaveLoadable, first create a struct that inherits
/// <see cref="ISaveState"/>. Then, implement ISaveLoadable with the typeparam
/// being whatever struct you had created.
///
/// <br/>
///
/// You could also create multiple serialization types for one class using
/// multiple ISaveLoadable interfaces and explicit interface implementations on
/// <see cref="SaveState"/>. See the example in the <see
/// cref="ISaveLoadable{T}"/> file.
///
/// <br/>
///
/// Serializing inherited types are also easy with this system. See the
/// implementation in <see cref="RangerTower"/> for a good example on how to do
/// it.
///
/// <br/>
///
/// Authors: Ryan Chang (2023)
/// </summary>
public interface ISaveLoadable<out T> where T : ISaveState
{
    /// <summary>
    /// Saves the object, returning a struct with its JSON data.
    /// </summary>
    /// <param name="prettyPrint">If true, pretty print the JSON.</param>
    /// <returns>A save struct.</returns>
    public abstract T SaveState();

    /// <summary>
    /// Evaluates whether or not this <see cref="ISaveLoadable{T}"/> can load from
    /// the provided <paramref name="state"/>.
    /// </summary>
    /// <param name="state">The <see cref="ISaveState"/> to evaluate.</param>
    /// <returns>True if can be loaded, false otherwise.</returns>
    public abstract bool CanLoadState(ISaveState state);

    /// <summary>
    /// Loads this object from <paramref name="state"/>.
    /// </summary>
    /// <param name="state">The <see cref="ISaveState"/> to evaluate.</param>
    public abstract void LoadState(ISaveState state);
}

/// <summary>
/// Note: I assume that you have basic knowledge of JSON serialization. If not,
/// see <see href="https://www.newtonsoft.com/json/help/html/Introduction.htm"/>
/// for an introduction on JSON serialization.
///
/// <br/>
///
/// A class or struct that contains the save information for the <see
/// cref="ISaveLoadable{T}"/> it belongs to. A class or struct that inherits
/// this interface is able to take advantage the static methods in <see
/// cref="SaveLoadExt"/>.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
public interface ISaveState
{

}

/// <summary>
/// A factory is often used when you are not really sure what <see
/// cref="ISaveState"/> type the JSON represents, or when you can't simply
/// create the <see cref="ISaveLoadable{T}"/> object using new(), such as
/// MonoBehaviors. A factory solves this by differentiating the <see
/// cref="ISaveState"/> type that the JSON is referencing as well as properly
/// constructing the <see cref="ISaveLoadable{T}"/> object so it can be used.
/// </summary>
/// <typeparam name="T">An <see cref="ISaveLoadable{T}"/> type.</typeparam>
public interface ISaveLoadableFactory<out T> where T : ISaveLoadable<ISaveState>
{
    /// <summary>
    /// Loads a <see cref="ISaveLoadable{T}"/> from the <paramref
    /// name="jsonString"/>. Note that the return value is just for reference
    /// checking: you should not use it to further instantiate the loaded
    /// object. For example, the <see cref="TowerFactory"/> loads, places, and
    /// properly sets up the tower, and returns a reference value for the
    /// completed tower.
    /// </summary>
    /// <param name="jsonString">The JSON representation of the
    /// ISaveState.</param>
    /// <returns>The created <see cref="ISaveLoadable{T}"/>.</returns>
    public abstract T LoadFromJson(string jsonString);
}

#region Converters
public class SaveLoadableConverter : JsonConverter<ISaveLoadable<ISaveState>>
{
    public override ISaveLoadable<ISaveState> ReadJson(JsonReader reader,
        Type objectType, ISaveLoadable<ISaveState> existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        throw new NotImplementedException(
            "Cannot load an ISaveLoadable. Use a factory instead.");
    }

    public override void WriteJson(JsonWriter writer,
        ISaveLoadable<ISaveState> value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.ToJsonString(serializer.Formatting == Formatting.Indented));
    }
}
#endregion