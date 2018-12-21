namespace com.mxgraph.io.gliffy.importer
{
    

	/// <summary>
	/// Enables post deserialization for classes that implement <seealso cref="PostDeserializer.PostDeserializable"/>
	/// </summary>
	public class PostDeserializer : TypeAdapterFactory
	{
		public interface PostDeserializable
		{
			void postDeserialize();
		}

		public virtual TypeAdapter<T> create<T>(Gson gson, TypeToken<T> type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.google.gson.TypeAdapter<T> delegate = gson.getDelegateAdapter(this, type);
			TypeAdapter<T> @delegate = gson.getDelegateAdapter(this, type);

			return new TypeAdapterAnonymousInnerClass(this, @delegate);
		}

		private class TypeAdapterAnonymousInnerClass : TypeAdapter<T>
		{
			private readonly PostDeserializer outerInstance;

			private TypeAdapter<T> @delegate;

			public TypeAdapterAnonymousInnerClass(PostDeserializer outerInstance, TypeAdapter<T> @delegate)
			{
				this.outerInstance = outerInstance;
				this.@delegate = @delegate;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(com.google.gson.stream.JsonWriter out, T value) throws java.io.IOException
			public virtual void write(JsonWriter @out, T value)
			{
				@delegate.write(@out, value);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T read(com.google.gson.stream.JsonReader in) throws java.io.IOException
			public virtual T read(JsonReader @in)
			{
				T obj = @delegate.read(@in);
				if (obj is PostDeserializable)
				{
					((PostDeserializable)obj).postDeserialize();
				}
				return obj;
			}
		}
	}
}