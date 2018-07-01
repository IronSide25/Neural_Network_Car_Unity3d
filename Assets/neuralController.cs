using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FitnessMeasure{
	distance,
	distance2byTime,
	distanceByTime
}
	

public class neuralController : MonoBehaviour {

	Rigidbody rigidbody;
	Rigidbody front, back, left, right;

	public FitnessMeasure fitnessMeasure;


	bool isColliding;

    public int timeScale;

	public int population;
	public static int staticPopulation;

    public double driveTime = 0;

	public float frontForce, backForce, leftForce, rightForce;

	public static int mutationRateStatic;
	public int mutationRate;
	public static int generation = 0;
	public double [] points;
	public double [] results;
	public double [] sensors;
	public static int currentNeuralNetwork = 0;

	public static float bestDistance = 0;


	Network [] networks;
	RaycastHit hit;

	Vector3 position;

	void mutationUpdate()
	{
		mutationRateStatic = mutationRate;
		//Debug.Log ("updated!");
	}

    // Use this for initialization
    void Start()
    {
		InvokeRepeating ("mutationUpdate", 1, 1);

		int[] parameters = { 5, 7, 2 };
		staticPopulation = population;

        Time.timeScale = timeScale;

        Debug.Log("Generation " + generation);

		rigidbody = GetComponent<Rigidbody>();
		front = GameObject.Find ("Front").GetComponent<Rigidbody> ();
		back = GameObject.Find ("Back").GetComponent<Rigidbody> ();
		left = GameObject.Find ("Left").GetComponent<Rigidbody> ();
		right = GameObject.Find ("Right").GetComponent<Rigidbody> ();

        results = new double[2];
        points = new double[population];
        sensors = new double[5];


	
       
        position = transform.position;
        networks = new Network[population];


        for (int i = 0; i < population; i++)
        {
			networks[i] = new Network(parameters);
        }

    }


	void FixedUpdate()
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		

		driveTime += Time.deltaTime;

		isColliding = false;


		if (transform.position.y > 5)
			OnCollisionEnter (null);



		//20 should be maximum force
		sensors[0] = transform.position.y / 5.0f;


		if (transform.eulerAngles.x < 180) {
		
			sensors [1] = transform.eulerAngles.x / 90.0f;
			sensors [2] = 0;
		
		} else {
			sensors [2] = -(transform.eulerAngles.x -360 ) /90.0f;
			sensors [1] = 0;
		}

		//distance to front
		if (transform.position.x > 0) {
			//Debug.Log ("!!");
			//Debug.Log(transform.position.x * 10000000);
			sensors [3] = transform.position.x * 2500000;
			sensors [4] = 0;
		} else {
			//Debug.Log ("??");
			sensors [4] = -(transform.position.x * 2500000);
			sensors [3] = 0;
		}

		//Debug.Log (sensors [0]);
		results = networks[currentNeuralNetwork].process(sensors);

		frontForce = (float)results [0] * 30;
		backForce = (float)results [1] * 30;

		front.AddRelativeForce (new Vector3(0,frontForce));
		right.AddRelativeForce (new Vector3(0,rightForce));
		left.AddRelativeForce (new Vector3(0,leftForce));
		back.AddRelativeForce (new Vector3(0,backForce));


	}


	//game over, friend :/
	void OnCollisionEnter (Collision col)
	{
		


		if(isColliding) return;
		isColliding = true;

		resetCarPosition();


		points [currentNeuralNetwork] = driveTime;
		

		driveTime = 0;

        //Debug.Log("network " + currentNeuralNetwork + " scored " + points[currentNeuralNetwork]);


		//now we reproduce
        if(currentNeuralNetwork == population-1)
        {
        double maxValue = points[0];
        int maxIndex = 0;

		//looking for the two best networks in the generation

        for(int i = 1; i < population; i++)
        {
            if (points[i] > maxValue)
            {
                maxIndex = i;
                maxValue = points[i];
            }
        }

        Debug.Log("first parent is " + maxIndex);

			if (points [maxIndex] > bestDistance) {
			
				bestDistance = (float)points [maxIndex];
			
			}

            points[maxIndex] = -10;

            Network mother = networks[maxIndex];


            maxValue = points[0];
            maxIndex = 0;

            for (int i = 1; i < population; i++)
            {
                if (points[i] > maxValue)
                {
                    maxIndex = i;
                    maxValue = points[i];
                }
            }

            Debug.Log("second parent is " + maxIndex);

            points[maxIndex] = -10;

            Network father = networks[maxIndex];


            for(int i = 0; i < population; i++)
            {
                points[i] = 0;
				//creating new generation of networks with random combinations of genes from two best parents
                networks[i] = new Network(father, mother);
            }

            generation++;
            Debug.Log("generation " + generation +" is born");

			//because we increment it at the beginning, that's why -1
            currentNeuralNetwork = -1;
        }

        currentNeuralNetwork++;

		//position reset is pretty important, don't forget it :*
        position = transform.position;
	}

	//TODO: sometimes the velocity is not reseted.. for some reason
    void resetCarPosition()
    {
        rigidbody.velocity = Vector3.zero;
        transform.position = new Vector3(0, 3, 0);
        transform.rotation = new Quaternion(0, 0, 0, 0);
		rigidbody.angularVelocity = new Vector3 (0, 0, 0);

    }





}

