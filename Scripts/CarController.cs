using UnityEngine;
using System.Collections;

namespace FunFant {
	public class CarController : MonoBehaviour {

		//Assign front wheels to 0 and 1, rear wheels to 2 and 3
		[SerializeField] private GameObject[] wheelMeshes = new GameObject[4];
		[SerializeField] private WheelCollider[] wheelColliders = new WheelCollider[4];

		[SerializeField] private GameObject steeringWheelMesh;

		//How much the steering wheel mesh is rotating
		[SerializeField] private float steeringWheelRatio = 1.0f;

		//Assign gameobject here to easily adjust the center of mass
		[SerializeField] private Transform centerOfMass;

		//Values for power and steering
		[SerializeField] private float maxSteer = 40f;
		[SerializeField] private float maxTorque = 100f;

		//Assign camera that follows the target and looks at car
		[SerializeField] private Camera followCamera;
		[SerializeField] private Transform cameraTarget;

		// Use this for initialization
		void Start () {
			Rigidbody rb = this.GetComponent<Rigidbody>();
			if(rb) {
				rb.centerOfMass = centerOfMass.localPosition;
			}
		}

		void Update () {
			if (Input.GetKeyDown (KeyCode.R)) {
				UnityEngine.SceneManagement.SceneManager.LoadScene (0);
			}
			if (Input.GetKeyDown (KeyCode.Escape)) {
				Application.Quit();
			}
		}

		private void FixedUpdate() {
			//Camera follow and turn
			if (followCamera) {
				Vector3 followTarget = new Vector3 (cameraTarget.transform.position.x, cameraTarget.transform.position.y, cameraTarget.transform.position.z);
				followCamera.transform.position = Vector3.Lerp (followCamera.transform.position, followTarget, Time.deltaTime * 2.5f);
				followCamera.transform.LookAt (this.transform);
			}
			//Car controls
			Steer(Input.GetAxis ("Horizontal"));

			Thrust(Input.GetAxis ("Vertical"));
		}

		void Steer(float value)	{
			//Collider angle
			float currentSteer = value * maxSteer;
			wheelColliders[0].steerAngle = currentSteer;
			wheelColliders[1].steerAngle = currentSteer;

			//Wheel meshes update
			for(int i = 0; i < 4; i++) {
				Quaternion rot;
				Vector3 pos;
				wheelColliders[i].GetWorldPose(out pos, out rot);
				wheelMeshes[i].transform.position = pos;
				wheelMeshes[i].transform.rotation = rot;
			}
			if (steeringWheelMesh) {
				steeringWheelMesh.transform.localEulerAngles = new Vector3(steeringWheelMesh.transform.localEulerAngles.x, steeringWheelMesh.transform.localEulerAngles.y, -currentSteer*steeringWheelRatio);
			
			}
		}

		void Thrust(float value) {
			float torque = value * maxTorque;
			wheelColliders[0].motorTorque = torque;
			wheelColliders[1].motorTorque = torque;
			wheelColliders[2].motorTorque = torque;
			wheelColliders[3].motorTorque = torque;
		}
	}
}
