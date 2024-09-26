using UnityEditor;


#pragma warning disable IDE0005
#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor
{

	[InitializeOnLoad]
	public static class LazyInitializer
	{
		static int counter;

		static LazyInitializer()
		{
			counter = -5;// start initializing five frames later
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			counter++;

			if (counter == 1)
				MainThreadDispatcher.Bump();
			else if (counter == 2)
				Logger.ELogger.Bump();
			else if (counter == 3)
				Monitor.Bump();
			else if (counter == 4)
				NetMQInitializer.Bump();
			else if (counter >= 5)
				EditorApplication.update -= OnUpdate;
		}

	}
}