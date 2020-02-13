using System;

namespace MAIN.Enums
{
    /// <summary>
    /// Enumerated type informing about the type of selected dll.
    /// </summary>
    [Flags]
    public enum DllVersion
    {
        VOID = 0, // no dll selected
        CPP_DLL = 1, // C++ dll algorithm
        ASM_DLL = 2 // assembler dll algorithm
    }
}
