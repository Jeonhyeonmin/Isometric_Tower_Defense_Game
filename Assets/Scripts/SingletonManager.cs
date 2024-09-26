using UnityEngine;

public class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindAnyObjectByType<T>();

				if (instance == null)
				{
					GameObject obj = new GameObject(typeof(T).Name, typeof(T));
					instance = obj.GetComponent<T>();
				}
			}
			return instance;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this as T;
			DontDestroyOnLoad(this.gameObject);
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		if (transform.parent != null && transform.root != null)
		{
			DontDestroyOnLoad(this.transform.root.gameObject);
		}
		else
		{
			DontDestroyOnLoad(this.gameObject);
		}
	}
}
