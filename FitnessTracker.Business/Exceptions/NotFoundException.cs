﻿namespace FitnessTracker.Business.Exceptions;

public class NotFoundException(string message) : Exception(message);