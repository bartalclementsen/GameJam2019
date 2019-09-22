using Core.Containers;
using Core.Loggers;
using Core.Mediators;
using Fading;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    public static IContainer Container { get; private set; }

    public static int NumberOfPlayers { get; set; }
    public static List<Color32> PlayerColors { get; } = new List<Color32>() {
        new Color32(0xea, 0x1b, 0x2c, 0xff),
        new Color32(0xd2, 0xa0, 0x2a, 0xff),
        new Color32(0x2b, 0x9a, 0x46, 0xff),
        new Color32(0xc1, 0x1e, 0x32, 0xff),
        new Color32(0xef, 0x41, 0x2a, 0xff),
        new Color32(0x00, 0xad, 0xd8, 0xff),
        new Color32(0xfd, 0xb7, 0x14, 0xff),
        new Color32(0x8f, 0x18, 0x38, 0xff),
        new Color32(0xf3, 0x6d, 0x24, 0xff),
        new Color32(0xe0, 0x17, 0x83, 0xff),
        new Color32(0xf9, 0x9d, 0x25, 0xff),
        new Color32(0xce, 0x8c, 0x2a, 0xff),
        new Color32(0x48, 0x77, 0x3c, 0xff),
        new Color32(0x00, 0x7c, 0xba, 0xff),
        new Color32(0x3e, 0xae, 0x49, 0xff),
        new Color32(0x00, 0x55, 0x8a, 0xff),
        new Color32(0x1a, 0x36, 0x68, 0xff)
    };


    public static int test { get; set; } 

    private static Core.Loggers.ILogger _logger;

    [RuntimeInitializeOnLoadMethod]
    private static void Main()
    {
        Debug.Log("Startup");

        Debug.Log("Starting bootstrap");

        Bootstrap();

        UnityEngine.Random.InitState(Convert.ToInt32(DateTime.Now.Ticks % int.MaxValue));

        _logger = Container.Resolve<ILoggerFactory>().Create(null);

        _logger.Log("Done bootstrap");
    }

    private static void Bootstrap()
    {
        ContainerBuilder containerBuilder = new ContainerBuilder();

        containerBuilder.Register<ILoggerFactory, LoggerFactory>();
        containerBuilder.RegisterSingleton<IMessenger, Messenger>();
        containerBuilder.RegisterSingleton<IFadeService, FadeService>();

        Container = containerBuilder.Build();
    }
}
