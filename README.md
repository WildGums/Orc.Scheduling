# Orc.Scheduling

[![Join the chat at https://gitter.im/WildGums/Orc.Scheduling](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/WildGums/Orc.Scheduling?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

![License](https://img.shields.io/github/license/wildgums/orc.Scheduling.svg)
![NuGet downloads](https://img.shields.io/nuget/dt/orc.Scheduling.svg)
![Version](https://img.shields.io/nuget/v/orc.Scheduling.svg)
![Pre-release version](https://img.shields.io/nuget/vpre/orc.Scheduling.svg)

Allows easily scheduling of (recurring) tasks inside applications.

# Quick introduction

Sometimes (recurring) tasks need to be scheduled inside an application. A few examples are maintenance tasks, check for updates, etc. This scheduling library is exposes a simple service to add scheduled tasks so you can easily schedule tasks without having to set up custom timers or remembering about the recurrance of a task.

This library does smart calculations to determine the next important step inside the schedule. It does not simple poll every second but wakes itself up (with a timer) on the next important step so it doesn't use any unnecessary resources.

# Controlling the scheduler

There are a few important methods to control the state of the scheduler. By default the scheduler is *enabled*.

## Starting the scheduler

	schedulingService.Start();

*Note that starting the service will immediately start tasks that are scheduled in the past*

## Stopping the scheduler

	schedulingService.Stop();

*Note that stopping the service will actively cancel all current active tasks*

# Scheduling tasks

The goal of this library is to make it as easy as possible to schedule simple tasks. Below are a few examples.

## Simple tasks

The following examples starts a task 5 minutes from now.

	var scheduledTask = new ScheduledTask
	{
		Name = "My task",
		Start = DateTime.Now.AddMinutes(5),
		Action = async () =>
		{
			/* logic here */
		}
	};

	schedulingService.AddScheduledTask(scheduledTask);

## Recurring tasks

The following examples starts a task 5 minutes from now, then restarts the task every 1 minute.

	var scheduledTask = new ScheduledTask
	{
		Name = "My task",
		Start = DateTime.Now.AddMinutes(5),
		Recurring = TimeSpan.FromMinutes(1),
		Action = async () =>
		{
			/* logic here */
		}
	};

	schedulingService.AddScheduledTask(scheduledTask);

## Canceling a running task

To cancel a task, you need a handle to a `RunningTask` instance. This can be retrieved by the `TaskStarted` event or the `RunningTasks` property. This example uses the latter, then cancels the task.
	
	var runningTask = (from task in schedulingService.RunningTasks
	                   where task.ScheduledTask.Name == "My task"
					   select task).FirstOrDefault();
	if (runningTask != null)
	{
		runningTask.CancellationTokenSource.Cancel();
	}