﻿using static Engine.GL;

namespace Engine
{
	public static class GLUtilities
	{
		public static unsafe void GenerateBuffers(out uint bufferId, out uint indexId)
		{
			uint[] buffers = new uint[2];

			fixed (uint* address = &buffers[0])
			{
				glGenBuffers(2, address);
			}

			bufferId = buffers[0];
			indexId = buffers[1];
		}

		public static unsafe void AllocateBuffers(int bufferSize, int indexSize, out uint bufferId,
			out uint indexBufferId, uint usage)
		{
			GenerateBuffers(out bufferId, out indexBufferId);

			// Note that buffer capacity should be given in bytes, while index capacity should be given in indexes
			// (i.e. unsigned shorts). This is meant to match how primitive buffers are created.
			glBindBuffer(GL_ARRAY_BUFFER, bufferId);
			glBufferData(GL_ARRAY_BUFFER, (uint)bufferSize, null, usage);

			glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBufferId);
			glBufferData(GL_ELEMENT_ARRAY_BUFFER, (uint)indexSize * sizeof(ushort), null, usage);
		}

		public static unsafe void DeleteBuffers(uint bufferId, uint indexId)
		{
			uint[] buffers =
			{
				bufferId,
				indexId
			};

			fixed (uint* address = &buffers[0])
			{
				glDeleteBuffers(2, address);
			}
		}
	}
}
