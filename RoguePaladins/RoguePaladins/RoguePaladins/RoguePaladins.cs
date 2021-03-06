﻿using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;


public class RoguePaladins : PhysicsGame
{
    const double Nopeus = 200;
    const double HyppyNopeus = 750;
    const int RUUDUN_KOKO = 40;

    PlatformCharacter pelaaja1;
    private PlatformCharacter vartija;

    Image pelaajanKuva = LoadImage("warrior");
    Image tahtiKuva = LoadImage("tahti");
    Image vartijaKuva = LoadImage("vartija");

    SoundEffect maaliAani = LoadSoundEffect("maali");
    

    public override void Begin()
    {
        Gravity = new Vector(0, -1000);

        LuoKentta();
        LisaaNappaimet();

        Camera.Follow(pelaaja1);
        Camera.ZoomFactor = 1.2;
        Camera.StayInLevel = true;
    }

    void LuoKentta()
    {
        TileMap kentta = TileMap.FromLevelAsset("kentta1");
        kentta.SetTileMethod('#', LisaaTaso);
        kentta.SetTileMethod('*', LisaaTahti);
        kentta.SetTileMethod('N', LisaaPelaaja);
        kentta.SetTileMethod('V', LisaaVartija);
        kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
        Level.CreateBorders();
        Level.Background.CreateGradient(Color.White, Color.SkyBlue);
    }

    void LisaaTaso(Vector paikka, double leveys, double korkeus)
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
        AddCollisionHandler(pelaaja1, "tahti", TormaaTahteen);
        Add(pelaaja1);
    }

    void LisaaVartija(Vector paikka, double leveys, double korkeus)
    {
        vartija = new Vihu(leveys, korkeus)
        {
            Position = paikka,
            Mass = 4.0,
            Image = vartijaKuva
        };
        AddCollisionHandler(pelaaja1, vartija, TormaaVihuun);
        Add(vartija);
        //Tehdään uudet satunnaisaivot, jotka liikkuvat nopeudella 200
        RandomMoverBrain satunnaisAivot = new RandomMoverBrain(200);

        //Ominaisuuksien muokkaaminen
        satunnaisAivot.ChangeMovementSeconds = 3;

        //Aivot käyttöön oliolle
        vartija.Brain = satunnaisAivot;
    }

    private void TormaaVihuun(IPhysicsObject collidingObject, IPhysicsObject otherObject)
    {
        throw new NotImplementedException();
    }

    void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -Nopeus);
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, Nopeus);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HyppyNopeus);

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");

        ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Liikuta, "Pelaaja liikkuu vasemmalle", pelaaja1, -Nopeus);
        ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Liikuta, "Pelaaja liikkuu oikealle", pelaaja1, Nopeus);
        ControllerOne.Listen(Button.A, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HyppyNopeus);

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
    }

}
