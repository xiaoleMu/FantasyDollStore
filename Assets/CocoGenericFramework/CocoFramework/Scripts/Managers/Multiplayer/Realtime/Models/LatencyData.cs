using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LatencyData {

	const int NUM_OF_SAMPLES=10;

	LinkedList<float> _samplesList;

	float _lastTime;

	public LatencyData(){
		Reset();
	}

	public void Reset(){
		_samplesList = new LinkedList<float>();
	}

	public void StartSample(){
		_lastTime = Time.timeSinceLevelLoad;
	}

	public void EndSample(){
		float timeDiff = (Time.timeSinceLevelLoad - _lastTime)/2;
		_samplesList.AddLast(timeDiff);
	}

	public bool IsFinished{
		get{return _samplesList.Count==NUM_OF_SAMPLES;}
	}

	public float CalcLatency(){

		float average=CalcAverage(_samplesList);

		LinkedListNode<float> node = _samplesList.First;

		while (node != null)
		{
			LinkedListNode<float> next = node.Next;

			float divation = Mathf.Abs((node.Value - average)/average);
			if(divation>0.35f)
				_samplesList.Remove(node);

			node = next;
		}

		return CalcAverage(_samplesList);

	}

	float CalcAverage(LinkedList<float> list){
		Debug.Log(" CalcAverage " );

		float average=0;

		for(LinkedListNode<float> it = list.First; it != null; it = it.Next){
			//Debug.Log(" ========== "+it.Value );
			average+=it.Value;
		}

		average/=list.Count;

		//Debug.Log(" average "+average );

		return average;

	}

}
