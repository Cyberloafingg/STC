using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public class JobTest : MonoBehaviour
{
    public int particleCount = 10000;
    public float particleSpeed = 10f;

    private NativeArray<Vector2> particlePositions;
    private NativeArray<Vector2> particleVelocities;
    private NativeArray<Vector2> particleAccelerations;

    private ParticleJob particleJob;
    private JobHandle particleJobHandle;

    private void Start()
    {
        // Initialize particle arrays
        particlePositions = new NativeArray<Vector2>(particleCount, Allocator.Persistent);
        particleVelocities = new NativeArray<Vector2>(particleCount, Allocator.Persistent);
        particleAccelerations = new NativeArray<Vector2>(particleCount, Allocator.Persistent);

        for (int i = 0; i < particleCount; i++)
        {
            particlePositions[i] = Random.insideUnitCircle * 10f;
            particleVelocities[i] = Random.insideUnitCircle * particleSpeed;
            particleAccelerations[i] = Vector2.zero;
        }
    }

    private void Update()
    {
        particleJob = new ParticleJob
        {
            position = particlePositions,
            velocity = particleVelocities,
            acceleration = particleAccelerations
        };

        particleJobHandle = particleJob.Schedule(particlePositions.Length, 128);
    }

    private void LateUpdate()
    {
        particleJobHandle.Complete();

        for (int i = 0; i < particleCount; i++)
        {
            transform.GetChild(i).position = particlePositions[i];
        }
    }

    private void OnDestroy()
    {
        particlePositions.Dispose();
        particleVelocities.Dispose();
        particleAccelerations.Dispose();
    }

    struct ParticleJob : IJobParallelFor
    {
        public NativeArray<Vector2> position;
        public NativeArray<Vector2> velocity;
        public NativeArray<Vector2> acceleration;

        public void Execute(int index)
        {
            Vector2 pos = position[index];
            Vector2 vel = velocity[index];
            Vector2 acc = acceleration[index];

            // Perform physics simulation
            vel += acc * 2;
            pos += vel * 2;

            velocity[index] = vel;
            position[index] = pos;
        }
    }
}