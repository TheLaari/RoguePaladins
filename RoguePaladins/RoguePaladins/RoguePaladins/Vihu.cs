using System;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

/// <summary>
/// Summary description for Class1
/// </summary>
class Vihu : PlatformCharacter
{
    private IntMeter elamaLaskuri = new IntMeter(3, 0, 3);
    public IntMeter ElamaLaskuri { get { return elamaLaskuri; } }

    public Vihu(double leveys, double korkeus)
        : base(leveys, korkeus)
    {
        elamaLaskuri.LowerLimit += delegate { this.Destroy(); };
    }
}