using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

//@author Joni Laari


public class RoguePaladins : PhysicsGame
    
{
    const double Nopeus = 200;
    const double HyppyNopeus = 500;
    const int RUUDUN_KOKO = 50;
    

    PlatformCharacter pelaaja1;

    Image pelaajanKuva = LoadImage("jeanele");
    Image tahtiKuva = LoadImage("portal");
    Image vartijaKuva = LoadImage("vartija");
    Image taustaKuva = LoadImage("level1background");


    SoundEffect maaliAani = LoadSoundEffect("maali");

    int kenttaNro = 1;

    public override void Begin()
    {
        Gravity = new Vector(0, -1000);


        MediaPlayer.Play("Honor_Bound");

        LuoKentta(kenttaNro);
        LisaaNappaimet();

        Camera.Follow(pelaaja1);
        Camera.ZoomFactor = 1.2;
        Camera.StayInLevel = true;

    }


    void LuoKentta(int kenttaNro) //ohjeet kentän rakentamiseen tekstistä
    {
        TileMap kentta = TileMap.FromLevelAsset("kentta" + kenttaNro);
        kentta.SetTileMethod('#', LisaaTaso);
        kentta.SetTileMethod('*', LisaaTahti);
        kentta.SetTileMethod('N', LisaaPelaaja);
        kentta.SetTileMethod('V', LisaaVartija);
        kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
        Level.CreateBorders();
        Level.Background.CreateGradient(Color.Black, Color.Green);
    }

    void LisaaTaso(Vector paikka, double leveys, double korkeus) //tasoelementti
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Color = Color.Green;
        Add(taso);
    }

    void LisaaTahti(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject tahti = PhysicsObject.CreateStaticObject(leveys, korkeus);
        tahti.IgnoresCollisionResponse = true;
        tahti.Position = paikka;
        tahti.Image = tahtiKuva;
        tahti.Tag = "tahti";
        Add(tahti);
    }

    void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
    {
        pelaaja1 = new PlatformCharacter(leveys, korkeus);
        pelaaja1.Position = paikka;
        pelaaja1.Mass = 4.0;
        pelaaja1.Image = pelaajanKuva;
        pelaaja1.Tag = "pelaaja1";
        AddCollisionHandler(pelaaja1, "tahti", TormaaTahteen);
        AddCollisionHandler(pelaaja1, "vartija", MeleeHyokkays);
        Add(pelaaja1);
    }




    void LisaaVartija(Vector paikka, double leveys, double korkeus)
    {
        Vihu vihuVartija = new Vihu(leveys, korkeus);

        vihuVartija.Position = paikka;
        vihuVartija.Mass = 4.0;
        vihuVartija.Image = vartijaKuva;
        vihuVartija.Tag = "vartija";
        Add(vihuVartija);

        vihuVartija.Brain = new FollowerBrain("pelaaja1");
    }

    void LisaaBossi(Vector paikka, double leveys, double korkeus)
    {
        Vihu vihuBoss = new Vihu(leveys, korkeus);

        vihuBoss.Position = paikka;
        vihuBoss.Mass = 4.0;
        vihuBoss.Image = vartijaKuva;
        vihuBoss.Tag = "pomo";
        Add(vihuBoss);

        vihuBoss.Brain = new RandomMoverBrain();
    }

    void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -Nopeus);
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, Nopeus);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Sankari hyppää", pelaaja1, HyppyNopeus);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Sankari hyppää", pelaaja1, HyppyNopeus);

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");

        ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Liikuta, "Sankari liikkuu vasemmalle", pelaaja1, -Nopeus);
        ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Liikuta, "Sankari liikkuu oikealle", pelaaja1, Nopeus);
        ControllerOne.Listen(Button.A, ButtonState.Pressed, Hyppaa, "Sankari hyppää", pelaaja1, HyppyNopeus);
        ControllerOne.Listen(Button.B, ButtonState.Pressed, Hyppaa, "Sankari hyökkää", pelaaja1, HyppyNopeus);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
    }

    void Liikuta(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Walk(nopeus);
    }

    void Hyppaa(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Jump(nopeus);
    }

    void TormaaTahteen(PhysicsObject hahmo, PhysicsObject tahti)
    {
        maaliAani.Play();
        MessageDisplay.Add("Keräsit tähden!");
        tahti.Destroy();
        kenttaNro++;
        SeuraavaKentta(kenttaNro);
    }

    void MeleeHyokkays(PhysicsObject hahmo, PhysicsObject vihollinen)
    {
        vihollinen.Destroy();
    }

    void SeuraavaKentta(int kenttaNro)
    {
        ClearAll();
        if (kenttaNro > 4) Exit(); //Tähän kolmosen kohdalle tulee kenttien lukumäärä
            else LuoKentta(kenttaNro);
        //Jotta tämä toimisi, pitää jokaisen kenttätiedoston nimen
        //olla muotoa kentta ja heti perään numero esimerkiksi kentta4.txt
    }

}
