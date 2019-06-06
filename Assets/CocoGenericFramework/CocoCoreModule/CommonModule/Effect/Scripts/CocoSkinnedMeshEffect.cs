using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale;

namespace CocoPlay
{
	[AddComponentMenu ("Coco Common/Global Common/Common/SkinnedMesh ParticleEffect")]
	[RequireComponent (typeof(ParticleSystem))]
	public class CocoSkinnedMeshEffect : GameView
	{
		public bool isLooped = false;
		public float loopIntervalMin = 7.0f;
		public float loopIntervalMax = 14.0f;
		float mLoopInterval;
		// emission
		public float emitDurationMin = 2.0f;
		public float emitDurationMax = 3.0f;
		float mEmitDuration;
		public float emitIntervalMin = 0.0f;
		public float emitIntervalMax = 1.0f;
		float mEmitInterval;
		public float emitCountRatioMin = 0.01f;
		public float emitCountRatioMax = 0.025f;
		float mEmitCount;
		// particle
		public float particleLifetimeMin = 2.0f;
		public float particleLifetimeMax = 5.0f;
		public float particleSizeMin = 0.1f;
		public float particleSizeMax = 0.6f;
		public AnimationCurve particleSizeCurveOverLifetime;
		List<ParticleSystem.Particle> mParticles;
		List<float> mParticleSizes;
		List<int> mParticleVerticeIndexes;
	
		// target
		SkinnedMeshRenderer m_SkinnedRenderer;

		public int maxEmitCount = 100;

		static public CocoSkinnedMeshEffect Create (SkinnedMeshRenderer _SkinnedRenderer)
		{
			CocoSkinnedMeshEffect effect = CocoLoad.Instantiate<CocoSkinnedMeshEffect> ("Coco_SkinnedMesh_Effect", _SkinnedRenderer.transform);
			effect.SetSkinnedMeshRenderer (_SkinnedRenderer);
			return effect;
		}

		public void  SetSkinnedMeshRenderer (SkinnedMeshRenderer _SkinnedRenderer)
		{
			m_SkinnedRenderer = _SkinnedRenderer;
		}

		protected override void Start ()
		{
			base.Start ();

			if (m_SkinnedRenderer == null)
				m_SkinnedRenderer = GetComponentInParent<SkinnedMeshRenderer> ();

			mLoopInterval = Random.Range (loopIntervalMin, loopIntervalMax);
			mEmitDuration = Random.Range (emitDurationMin, emitDurationMax);
			mEmitInterval = 0.0f;
			mParticles = new List<ParticleSystem.Particle> ();
			mParticleSizes = new List<float> ();
			mParticleVerticeIndexes = new List<int> ();
		}

		protected override void OnRegister ()
		{
			base.OnRegister ();
		}

		void Update ()
		{
			if (m_SkinnedRenderer == null)
				return;

			mEmitDuration -= Time.deltaTime;
			if (mEmitDuration > 0.0f) {
				mEmitInterval -= Time.deltaTime;
				if (mEmitInterval <= 0.0f) {
					EmitParticles ();
					mEmitInterval = Random.Range (emitIntervalMin, emitIntervalMax);
				}
			}

			bool hasChanged = UpdateParticles (Time.deltaTime);

			if (!hasChanged) {
				if (!isLooped) {
					Destroy (gameObject);
				} else {
					mLoopInterval -= Time.deltaTime;
					if (mLoopInterval <= 0.0f) {
						mLoopInterval = Random.Range (loopIntervalMin, loopIntervalMax);
						mEmitDuration = Random.Range (emitDurationMin, emitDurationMax);
					}
				}
			}
		}

		private void EmitParticles ()
		{
//			return;
			Mesh mesh = new Mesh ();
			m_SkinnedRenderer.BakeMesh (mesh);
			Vector3 meshPos = mesh.bounds.center;
			Vector3 scale = transform.lossyScale;
			transform.localPosition = new Vector3 (meshPos.x / scale.x, meshPos.y / scale.y, meshPos.z / scale.z);
			Vector3[] vertices = mesh.vertices;
			//Vector3[] nromals = mesh.normals;
			int mEmitCount = (int)Random.Range (vertices.Length * emitCountRatioMin, vertices.Length * emitCountRatioMax);

			mEmitCount = Mathf.Clamp (mEmitCount, 0, maxEmitCount);

			if (mEmitCount <= 0)
				mEmitCount = 1;
			else if (mEmitCount > vertices.Length)
				mEmitCount = vertices.Length;

//			mEmitCount = Mathf.Clamp (mEmitCount, 0, 8);//limit particle num
		
			for (int i = 0; i < mParticles.Count && mEmitCount > 0; i++) {
				if (mParticles [i].remainingLifetime <= 0) {
					int verticeIndex = Random.Range (0, vertices.Length);
					mParticles [i] = GenerateParticle (vertices [verticeIndex] - meshPos);
					mParticleSizes [i] = Random.Range (particleSizeMin, particleSizeMax);
					mParticleVerticeIndexes [i] = verticeIndex;
				
					mEmitCount--;
				}
			}
		
			for (int i = 0; i < mEmitCount; i++) {
				int verticeIndex = Random.Range (0, vertices.Length);
				mParticles.Add (GenerateParticle (vertices [verticeIndex] - meshPos));
				mParticleSizes.Add (Random.Range (particleSizeMin, particleSizeMax));
				mParticleVerticeIndexes.Add (verticeIndex);
			}

			Destroy (mesh);
		}

		private bool UpdateParticles (float pDelta)
		{
			bool ParticlesChanged = false;
			Vector3 meshPos = Vector3.zero;
			Mesh mesh = null;
		
			for (int i = 0; i < mParticles.Count; i++) {
				if (mParticles [i].remainingLifetime > 0.0f) {
					if (mesh == null) {
						mesh = new Mesh ();
						m_SkinnedRenderer.BakeMesh (mesh);
						meshPos = mesh.bounds.center;
						Vector3 scale = transform.lossyScale;
						transform.localPosition = new Vector3 (meshPos.x / scale.x, meshPos.y / scale.y, meshPos.z / scale.z);
					}
				
					ParticlesChanged = true;
					ParticleSystem.Particle particle = mParticles [i];
					int verticeIndex = mParticleVerticeIndexes [i];
					if (verticeIndex >= 0 && verticeIndex < mesh.vertices.Length)
						particle.position = mesh.vertices [verticeIndex] - meshPos;
					float val = particleSizeCurveOverLifetime.Evaluate ((mParticles [i].startLifetime - mParticles [i].remainingLifetime) / mParticles [i].startLifetime);
					particle.size = mParticleSizes [i] * val;
					particle.color = new Color (1.0f, 1.0f, 1.0f, val);
					particle.remainingLifetime -= pDelta;
					mParticles [i] = particle;
				}
			}
		
			if (mesh != null)
				Destroy (mesh);
		
			if (ParticlesChanged)
				GetComponent <ParticleSystem> ().SetParticles (mParticles.ToArray (), mParticles.Count);
		
			return ParticlesChanged;
		}

		private ParticleSystem.Particle GenerateParticle (Vector3 pPos)
		{
			ParticleSystem.Particle particle = new ParticleSystem.Particle ();
			
			particle.position = pPos;
			particle.startLifetime = Random.Range (particleLifetimeMin, particleLifetimeMax);
			particle.remainingLifetime = particle.startLifetime;
			particle.size = 0.0f;
			particle.rotation = Random.Range (-180.0f, 180.0f);
			particle.color = Color.white;
		
			return particle;
		}
	}
}
