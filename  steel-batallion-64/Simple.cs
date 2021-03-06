//If you want a quick overview of how the configuration system works, take a look at SolExodus.cs
//This example was meant to recreate the functionality I displayed for the system in the original release
//however that also means that it is actually pretty complicated.

using System;
using Microsoft.DirectX.DirectInput;
namespace SBC{
public class DynamicClass
{
String debugString = "";
SteelBattalionController controller;
vJoy joystick;
bool acquired;
bool mouseStarted = false;
int desiredX;
int desiredY;
int currentX = -1;
int currentY = -1;

const int refreshRate = 50;//number of milliseconds between call to mainLoop


	//this gets called once by main program
    public void Initialize()
    {
        int baseLineIntensity = 1;//just an average value for LED intensity
        int emergencyLightIntensity = 15;//for stuff like eject,cockpit Hatch,Ignition, and Start

		controller = new SteelBattalionController();
		controller.Init(50);//50 is refresh rate in milliseconds
		//set all buttons by default to light up only when you press them down

		for(int i=4;i<4+30;i++)
		{
			if (i != (int)ButtonEnum.Eject)//excluding eject since we are going to flash that one
			controller.AddButtonLightMapping((ButtonEnum)(i-1),(ControllerLEDEnum)(i),true,baseLineIntensity);
		}
		
         controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch,            true, 3,    Microsoft.DirectX.DirectInput.Key.A, true);//last true means if you hold down the button,		
		 controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF1,				true, 3,    Microsoft.DirectX.DirectInput.Key.B, true);
		 controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon,Microsoft.DirectX.DirectInput.Key.C, true);
		 joystick = new vJoy();
		 acquired = joystick.acquireVJD(1);
		 joystick.resetAll();//have to reset before we use it
		
		joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_SL1);			
		joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_X);
		joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_Y);
		joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_Z);//throttle
		joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_RZ);
		joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_SL0);		
		joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_RX);				
		joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_RY);	
		 
		 
		desiredX = currentX = controller.AimingX;
		desiredY = currentY = controller.AimingY;
		 
	}
	
	//this is necessary, as main program calls this to know how often to call mainLoop
	public int getRefreshRate()
	{
		return refreshRate;
	}
	
	private uint getDegrees(double x,double y)
	{
		uint temp = (uint)(System.Math.Atan(y/x)* (180/Math.PI));
		if(x < 0)
			temp +=180;
		if(x > 0 && y < 0)
			temp += 360;
			
		temp += 90;//origin is vertical on POV not horizontal
			
		if(temp > 360)//by adding 90 we may have gone over 360
			temp -=360;
		
		temp*=100;
		
		if (temp > 35999)
			temp = 35999;
		if (temp < 0)
			temp = 0;
		return temp;
	}
	
//	private int scaledValue(int min, int middle, int max, int deadZone)

	

	//this gets called once every refreshRate milliseconds by main program
	public void mainLoop()
	{
	float lowValue = 124;
	float highValue = 255;
	int gearValue;
	
	int numPixels = 600;
	
	desiredX = (int)((controller.AimingX / 1024.0) * numPixels);
	desiredY = (int)((controller.AimingY / 1024.0) * numPixels);

	
	if (controller.GearLever == -2)//R
		gearValue = -255;
	else if (controller.GearLever == -1)//N
		gearValue = 124;		
	else
	{
		gearValue = (int)(lowValue + (highValue - lowValue)*((controller.GearLever-1.0)/4.0));
	}
	
		debugString = "not in";
		if((bool)controller.GetButtonState(ButtonEnum.RightJoyMainWeapon))
		{
			debugString = "RightJoyMainWeapon";
			desiredX = currentX;
			desiredY = currentY;
		}
		if((bool)controller.GetButtonState(ButtonEnum.RightJoyLockOn))
		{
		int deltaX = desiredX - currentX;
		int deltaY = desiredY - currentY;
		currentX = desiredX;
		currentY = desiredY;

		//debugString = "in";
		InputSimulator.MoveMouseBy(deltaX,deltaY);
		}
		/*
		joystick.setAxis(1,controller.GearLever,HID_USAGES.HID_USAGE_SL1);	
		
		//joystick.setAxis(1,controller.AimingX,HID_USAGES.HID_USAGE_X);
		//joystick.setAxis(1,controller.AimingY,HID_USAGES.HID_USAGE_Y);

		
		
		
		joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_X);
		joystick.setAxis(1,32768/2,HID_USAGES.HID_USAGE_Y);
		
		joystick.setAxis(1,-1*(controller.RightPedal - controller.MiddlePedal),HID_USAGES.HID_USAGE_Z);//throttle
		joystick.setAxis(1,controller.RotationLever,HID_USAGES.HID_USAGE_RZ);
		joystick.setAxis(1,controller.SightChangeX,HID_USAGES.HID_USAGE_SL0);		
		joystick.setAxis(1,controller.SightChangeY,HID_USAGES.HID_USAGE_RX);				
		joystick.setAxis(1,controller.LeftPedal,HID_USAGES.HID_USAGE_RY);		*/				

		
		joystick.setContPov(1,getDegrees(controller.SightChangeX,controller.SightChangeY),1);


		for(int i=1;i<=32;i++)
		{
			joystick.setButton((bool)controller.GetButtonState(i-1),(uint)1,(char)(i-1));
		}
		
		joystick.sendUpdate(1);

	}
	
	//new necessary function used for debugging purposes
	public String getDebugString()
	{
		return debugString;
	}
	
	//this gets called at the end of the program and must be present, as it cleans up resources
	public void shutDown()
	{
		controller.UnInit();
		joystick.Release(1);
	}
	
}
}