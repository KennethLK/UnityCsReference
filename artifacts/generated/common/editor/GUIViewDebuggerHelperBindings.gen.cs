// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using scm=System.ComponentModel;
using uei=UnityEngine.Internal;
using RequiredByNativeCodeAttribute=UnityEngine.Scripting.RequiredByNativeCodeAttribute;
using UsedByNativeCodeAttribute=UnityEngine.Scripting.UsedByNativeCodeAttribute;

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using IntPtr = System.IntPtr;
using System;
namespace UnityEditor
{


[RequiredByNativeCode]
    internal struct IMGUIClipInstruction
    {
        public Rect screenRect;
        public Rect unclippedScreenRect;
        public Vector2 scrollOffset;
        public Vector2 renderOffset;
        public bool resetOffset;
        public int level;

        public StackFrame[] pushStacktrace;
        public StackFrame[] popStacktrace;
    }


[RequiredByNativeCode]
    internal struct IMGUILayoutInstruction
    {
        public int level;
        public Rect unclippedRect;

        public int marginLeft;
        public int marginRight;
        public int marginTop;
        public int marginBottom;

        public GUIStyle style;


        public StackFrame[] stack;

        public int isGroup;
        public int isVertical;
    }


internal enum InstructionType
{
    kStyleDraw = 1,
    kClipPush = 2,
    kClipPop = 3,
    kLayoutBeginGroup = 4,
    kLayoutEndGroup = 5,
    kLayoutEntry = 6
}

[RequiredByNativeCode]
    struct IMGUIInstruction
    {
        public InstructionType type;
        public int level;
        public Rect unclippedRect;
        public StackFrame[] stack;

        public int typeInstructionIndex;
    }


internal partial class GUIViewDebuggerHelper
{
    static internal void GetViews(List<GUIView> views)
        {
            GetViewsInternal(views);
        }
    
    
    [UnityEngine.Scripting.GeneratedByOldBindingsGeneratorAttribute] // Temporarily necessary for bindings migration
    [System.Runtime.CompilerServices.MethodImplAttribute((System.Runtime.CompilerServices.MethodImplOptions)0x1000)]
    extern private static  void GetViewsInternal (object views) ;

    [UnityEngine.Scripting.GeneratedByOldBindingsGeneratorAttribute] // Temporarily necessary for bindings migration
    [System.Runtime.CompilerServices.MethodImplAttribute((System.Runtime.CompilerServices.MethodImplOptions)0x1000)]
    extern internal static  void DebugWindow (GUIView view) ;

    [UnityEngine.Scripting.GeneratedByOldBindingsGeneratorAttribute] // Temporarily necessary for bindings migration
    [System.Runtime.CompilerServices.MethodImplAttribute((System.Runtime.CompilerServices.MethodImplOptions)0x1000)]
    extern internal static  void StopDebugging () ;

    [UnityEngine.Scripting.GeneratedByOldBindingsGeneratorAttribute] // Temporarily necessary for bindings migration
    [System.Runtime.CompilerServices.MethodImplAttribute((System.Runtime.CompilerServices.MethodImplOptions)0x1000)]
    extern internal static  int GetInstructionCount () ;

    public static Rect GetRectFromInstruction (int instructionIndex) {
        Rect result;
        INTERNAL_CALL_GetRectFromInstruction ( instructionIndex, out result );
        return result;
    }

    [UnityEngine.Scripting.GeneratedByOldBindingsGeneratorAttribute] // Temporarily necessary for bindings migration
    [System.Runtime.CompilerServices.MethodImplAttribute((System.Runtime.CompilerServices.MethodImplOptions)0x1000)]
    private extern static void INTERNAL_CALL_GetRectFromInstruction (int instructionIndex, out Rect value);
    [UnityEngine.Scripting.GeneratedByOldBindingsGeneratorAttribute] // Temporarily necessary for bindings migration
    [System.Runtime.CompilerServices.MethodImplAttribute((System.Runtime.CompilerServices.MethodImplOptions)0x1000)]
    extern public static  GUIStyle GetStyleFromInstruction (int instructionIndex) ;

    internal static GUIContent GetContentFromInstruction(int instructionIndex)
        {
            var content = new GUIContent();
            content.text = GetContentTextFromInstruction(instructionIndex);
            content.image = GetContentImageFromInstruction(instructionIndex);
            return content;
        }
    
    
    [UnityEngine.Scripting.GeneratedByOldBindingsGeneratorAttribute] // Temporarily necessary for bindings migration
    [System.Runtime.CompilerServices.MethodImplAttribute((System.Runtime.CompilerServices.MethodImplOptions)0x1000)]
    extern private static  string GetContentTextFromInstruction (int instructionIndex) ;

    [UnityEngine.Scripting.GeneratedByOldBindingsGeneratorAttribute] // Temporarily necessary for bindings migration
    [System.Runtime.CompilerServices.MethodImplAttribute((System.Runtime.CompilerServices.MethodImplOptions)0x1000)]
    extern private static  Texture GetContentImageFromInstruction (int instructionIndex) ;

    [UnityEngine.Scripting.GeneratedByOldBindingsGeneratorAttribute] // Temporarily necessary for bindings migration
    [System.Runtime.CompilerServices.MethodImplAttribute((System.Runtime.CompilerServices.MethodImplOptions)0x1000)]
    extern public static  StackFrame[] GetManagedStackTrace (int instructionIndex) ;

    internal static void GetClipInstructions(List<IMGUIClipInstruction> clipInstructions)
        {
            GetClipInstructionsInternal(clipInstructions);
        }
    
    
    [UnityEngine.Scripting.GeneratedByOldBindingsGeneratorAttribute] // Temporarily necessary for bindings migration
    [System.Runtime.CompilerServices.MethodImplAttribute((System.Runtime.CompilerServices.MethodImplOptions)0x1000)]
    extern private static  void GetClipInstructionsInternal (object clipInstructions) ;

    internal static void GetLayoutInstructions(List<IMGUILayoutInstruction> layoutInstructions)
        {
            GetLayoutInstructionsInternal(layoutInstructions);
        }
    
    
    [UnityEngine.Scripting.GeneratedByOldBindingsGeneratorAttribute] // Temporarily necessary for bindings migration
    [System.Runtime.CompilerServices.MethodImplAttribute((System.Runtime.CompilerServices.MethodImplOptions)0x1000)]
    extern private static  void GetLayoutInstructionsInternal (object layoutInstructions) ;

    internal static void GetUnifiedInstructions(List<IMGUIInstruction> layoutInstructions)
        {
            GetUnifiedInstructionsInternal(layoutInstructions);
        }
    
    
    [UnityEngine.Scripting.GeneratedByOldBindingsGeneratorAttribute] // Temporarily necessary for bindings migration
    [System.Runtime.CompilerServices.MethodImplAttribute((System.Runtime.CompilerServices.MethodImplOptions)0x1000)]
    extern private static  void GetUnifiedInstructionsInternal (object instructions) ;

}


} 
