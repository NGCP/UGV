# -*- coding: utf-8 -*-
"""
Created on Thu Jan 11 14:23:34 2018

@author: epowertheif
"""
import pygame 
import math
pygame.init()
white = (255,255,255)
black = (0,0,0)
red = (255,0,0)
green = (0,255,0)
lightgreen = (0,150,0)
lightred = (150,0,0)
blue = (0,0,255)
width = 1200
height = 650
smallfont = pygame.font.SysFont("Arial",25)
medfont = pygame.font.SysFont("Arial",50)
largefont = pygame.font.SysFont("Arial",80)
pygame.display.set_caption("Obstacle avoidance is not working")
gameDisplay = pygame.display.set_mode((width,height))
clock = pygame.time.Clock()
maxspeed = 50 #Max speed is 2 M/s
maxrotation = 90 #Tweak this
MaxSensor = 100 #10 Meter Range
FPS = 60
#SCALE IS EVERY 10 UNITS IS A METER 

class NGVRobot(object):
    def __init__(self,sizex,sizey,posx,posy):
        self.sizex = sizex
        self.sizey = sizey 
        self.posx = posx 
        self.posy = posy 
        self.vectorRotation = 0 
        self.vectorSpeed = 0
        self.angle = 0
        self.point1 = [posx+sizex/2,posy+sizey/2]
        self.point2 = [posx+sizex/2,posy-sizey/2]
        self.point3 = [posx-sizex/2,posy+sizey/2]
        self.point4 = [posx-sizex/2,posy-sizey/2]
        self.Lidar = [posx,posy-sizey/2]
    def Draw(self):
        pygame.draw.line(gameDisplay,black,self.point1,self.point2)
        pygame.draw.line(gameDisplay,black,self.point1,self.point3)
        pygame.draw.line(gameDisplay,black,self.point4,self.point2)
        pygame.draw.line(gameDisplay,black,self.point4,self.point3)
        pygame.draw.circle(gameDisplay,black,[int(self.Lidar[0]),int(self.Lidar[1])],2)
    def Rotate(self,angle):
        self.angle = angle
        self.point1 = [self.posx+self.sizex/2,self.posy+self.sizey/2]
        self.point2 = [self.posx+self.sizex/2,self.posy-self.sizey/2]
        self.point3 = [self.posx-self.sizex/2,self.posy+self.sizey/2]
        self.point4 = [self.posx-self.sizex/2,self.posy-self.sizey/2]
        self.Lidar = [self.posx,self.posy-self.sizey/2]
        self.point1 = self.Spin(self.point1)
        self.point2 = self.Spin(self.point2)
        self.point3 = self.Spin(self.point3)
        self.point4 = self.Spin(self.point4)
        self.Lidar = self.Spin(self.Lidar)
    def Spin(self, point):
        sin = math.sin(self.angle * math.pi/180)
        cos = math.cos(self.angle * math.pi/180)
        point[0] -= self.posx
        point[1] -= self.posy
        newx = point[0] * cos - point[1] * sin
        newy = point[0] * sin + point[1] * cos
        point[0] = newx + self.posx 
        point[1] = newy + self.posy
        return point 
    def Speed(self):
        self.posx += self.speedvector * math.sin(self.angle * math.pi/180)
        self.posy -= self.speedvector * math.cos(self.angle * math.pi/180)
    def Movement(self,Vector):
        angleaddition  =  -self.angle + Vector[1] 
        self.speedvector = (Vector[0] * math.sin(angleaddition*math.pi /180))/FPS #Vector[1] Needs to be fixed it is not correct
        #add decision for smallest turn
        #print(angleaddition)
        self.angle -= (maxrotation * math.cos(angleaddition * math.pi/180)) /FPS #changed Vector[0] to max rotation
        self.angle = self.angle % 360
        self.Rotate(self.angle)
        self.Speed()
    def WayPointVector(self,Target):
        WayPointx = self.posx - Target.x
        WayPointy = self.posy - Target.y
        Magnitude = math.sqrt(WayPointx**2 + WayPointy**2)
        if(Magnitude > maxspeed):
            tempx = WayPointx/Magnitude
            tempy = WayPointy/Magnitude
            Magnitude = math.sqrt(tempx**2+tempy**2) * maxspeed
        angle = math.atan2(WayPointy,WayPointx) * 180 / math.pi
        return (Magnitude,angle)
    def LidarScan(self,ObstacleArray):
        LidarScan = self.ProduceLidarPoints()
        ObstaclesDetectedPoints = []
        #CHECK EVERYTHING INCLUDE REFERENCE ANGLE IN THIS
        for Points in LidarScan:
            TempShortestMagnitude = []
            ShortestMagnitude =  MaxSensor + 1
            check = False
            for Obstacle in ObstacleArray:
                Rectangle = Obstacle.ClippingArea()
                TempLidar = cohensutherland(Rectangle[0], Rectangle[3], Rectangle[2], Rectangle[1], self.Lidar[0], self.Lidar[1], Points[0], Points[1])
                #print(TempLidar)
                if(TempLidar == (None, None, None, None)):
                    continue
                Value1 = math.sqrt((TempLidar[0]-self.Lidar[0])**2 + (self.Lidar[1]-TempLidar[1])**2)
                Value2 = math.sqrt((TempLidar[2]-self.Lidar[0])**2 + (self.Lidar[1]-TempLidar[3])**2)
                if(Value1<Value2):
                    TempShortestMagnitude.append([TempLidar[0]-self.Lidar[0],self.Lidar[1] -TempLidar[1]])
                    pygame.draw.line(gameDisplay,red,self.Lidar,[TempLidar[0],TempLidar[1]])
                else:
                    TempShortestMagnitude.append([TempLidar[2]-self.Lidar[0],self.Lidar[1] -TempLidar[3]])
                    #pygame.draw.line(gameDisplay,red,self.Lidar,[TempLidar[2],TempLidar[3]])
                check = True
            for Magnitude in TempShortestMagnitude:
                Temp = math.sqrt(Magnitude[0]**2 + Magnitude[1]**2)
                if(Temp < ShortestMagnitude):
                    ShortestReading = [Magnitude[0],Magnitude[1]]
                    ShortestMagnitude = Temp
            if(check):
                Magn = math.sqrt(ShortestReading[0]**2 + ShortestReading[1]**2)
                Angle = round((math.atan2(ShortestReading[1],ShortestReading[0])*180/math.pi + self.angle - 90)*-1,2) 
                if(Angle < -135):
                    Angle += 360
                if(Angle > 135):
                    Angle -= 360 
                ObstaclesDetectedPoints.append([Magn,Angle])
        return ObstaclesDetectedPoints
            
    def ProduceLidarPoints(self):
        #Edit If u want
        LidarAngle = -270##
        LidarPoints = []
        #Edit for Lidar Range
        InitialX = MaxSensor*math.cos((self.angle+45) *math.pi/180) + self.Lidar[0]##
        InitialY = MaxSensor*math.sin((self.angle+45)* math.pi/180) + self.Lidar[1]##
        while LidarAngle <= 0:
            X = (InitialX-self.Lidar[0])*math.cos(LidarAngle*math.pi/180) - (InitialY-self.Lidar[1]) * math.sin(LidarAngle*math.pi/180) + self.Lidar[0]
            Y = (InitialX - self.Lidar[0])*math.sin(LidarAngle*math.pi/180) + (InitialY - self.Lidar[1]) * math.cos(LidarAngle*math.pi/180) + self.Lidar[1]
            LidarPoints.append([X,Y])
            LidarAngle += .25
        for Points in LidarPoints:
            pygame.draw.circle(gameDisplay,black,[int(Points[0]),int(Points[1])],1)
        return LidarPoints
class VectorAddingAvoidance(NGVRobot):
    def VectorAvoidance(self,ObstacleArray,Target):
        LidarScan = self.LidarScanMaxDistance(ObstacleArray)
        VectorAdd = [0,0]
        #print(LidarScan)
        #print(VectorAdd)
        Gain = .05
        WayPointGain = .2
        for Points in LidarScan:
            Vx = (Points[0] * math.cos(Points[1]*math.pi/180))
            Vy = (Points[0] * math.sin(Points[1]*math.pi/180)) 
            VectorAdd[0] += Vx * Gain
            VectorAdd[1] += Vy * Gain 
        WayPoint = self.WayPointVector(Target)
        VectorAdd[0] = round(VectorAdd[0],0)
        VectorAdd[1] = round(VectorAdd[1],0)
        VectorAdd[0] += WayPoint[0] * math.cos(WayPoint[1]*math.pi/180) *WayPointGain
        VectorAdd[1] += WayPoint[0] * math.sin(WayPoint[1]*math.pi/180)  *WayPointGain
        Magn = math.sqrt(VectorAdd[0]**2+VectorAdd[1]**2)
        VectorAdd[0] = VectorAdd[0]/Magn * maxspeed
        VectorAdd[1] = VectorAdd[1]/Magn * maxspeed 
        Magn = math.sqrt(VectorAdd[0]**2+VectorAdd[1]**2)
        Angle = math.atan2(VectorAdd[1],VectorAdd[0]) * 180/ math.pi
        return [Magn,Angle]
        
        
    def LidarScanMaxDistance(self,ObstacleArray):
        LidarScan = self.ProduceLidarPoints()
        ObstaclesDetectedPoints = []
        #CHECK EVERYTHING INCLUDE REFERENCE ANGLE IN THIS
        for Points in LidarScan:
            TempShortestMagnitude = []
            ShortestMagnitude =  MaxSensor + 1
            check = False
            for Obstacle in ObstacleArray:
                Rectangle = Obstacle.ClippingArea()
                TempLidar = cohensutherland(Rectangle[0], Rectangle[3], Rectangle[2], Rectangle[1], self.Lidar[0], self.Lidar[1], Points[0], Points[1])
                #print(TempLidar)
                if(TempLidar == (None, None, None, None)):
                    continue
                Value1 = math.sqrt((TempLidar[0]-self.Lidar[0])**2 + (self.Lidar[1]-TempLidar[1])**2)
                Value2 = math.sqrt((TempLidar[2]-self.Lidar[0])**2 + (self.Lidar[1]-TempLidar[3])**2)
                if(Value1<Value2):
                    TempShortestMagnitude.append([TempLidar[0]-self.Lidar[0],self.Lidar[1] -TempLidar[1]])
                    pygame.draw.line(gameDisplay,red,self.Lidar,[TempLidar[0],TempLidar[1]])
                else:
                    TempShortestMagnitude.append([TempLidar[2]-self.Lidar[0],self.Lidar[1] -TempLidar[3]])
                    #pygame.draw.line(gameDisplay,red,self.Lidar,[TempLidar[2],TempLidar[3]])
                check = True
            for Magnitude in TempShortestMagnitude:
                Temp = math.sqrt(Magnitude[0]**2 + Magnitude[1]**2)
                if(Temp < ShortestMagnitude):
                    ShortestReading = [Magnitude[0],Magnitude[1]]
                    ShortestMagnitude = Temp
            if(check):
                Magn = math.sqrt(ShortestReading[0]**2 + ShortestReading[1]**2) #HERE IS AN ISSUE LOOK HERE
                Angle = round((math.atan2(ShortestReading[1],ShortestReading[0])*180/math.pi + self.angle - 90),2) 
                if(Angle < -135):
                    Angle += 360
                if(Angle > 135): ###### IVE BEEN MESSING AROUND WITH THIS AREA CHECK IT
                    Angle -= 360 
                ObstaclesDetectedPoints.append([Magn,Angle])
            else: # AND HERE #Issue is here i think #Its fixed weeee
                W = round((math.atan2(self.Lidar[1]-Points[1],Points[0]-self.Lidar[0])*180/math.pi+self.angle - 90),2)
                if(W < -135):
                    W += 360
                if(W > 135):
                    W -= 360
                ObstaclesDetectedPoints.append([MaxSensor,W])
        LidarAngle1 = -179.75
        while LidarAngle1 < -135:
           ObstaclesDetectedPoints.append([MaxSensor,LidarAngle1])
           LidarAngle1 += .25
        LidarAngle1 = 135.25
        while LidarAngle1 <= 180:
            ObstaclesDetectedPoints.append([MaxSensor,LidarAngle1])
            LidarAngle1 += .25
        #print(ObstaclesDetectedPoints)
        return ObstaclesDetectedPoints
            
            
class Target(object):
    def __init__(self,posx,posy):
        self.x = posx
        self.y = posy
    def Draw(self):
        pygame.draw.circle(gameDisplay,green,[self.x,self.y],5)
        
class obstacle(object):
    def __init__(self,posx,posy,sizex,sizey):
        self.posx = posx
        self.posy = posy
        self.sizex = sizex
        self.sizey = sizey
    def Draw(self):
        pygame.draw.rect(gameDisplay,blue,[self.posx-self.sizex/2,self.posy-self.sizey/2,self.sizex,self.sizey],1)
    def ClippingArea(self):
        xmin = self.posx - self.sizex/2
        ymax = self.posy - self.sizey/2
        xmax = self.posx + self.sizex/2
        ymin = self.posy + self.sizey/2
        return [xmin,ymax,xmax,ymin]

INSIDE,LEFT, RIGHT, LOWER, UPPER = 0,1, 2, 4, 8
    
def cohensutherland(xmin, ymax, xmax, ymin, x1, y1, x2, y2):
    k1 = _getclip(x1, y1,xmin,xmax,ymin,ymax)
    k2 = _getclip(x2, y2,xmin,xmax,ymin,ymax)
    #examine non-trivially outside points
    #bitwise OR |
    while (k1 | k2) != 0: # if both points are inside box (0000) , ACCEPT trivial whole line in box

        # if line trivially outside window, REJECT
        if (k1 & k2) != 0: #bitwise AND &
            #if dbglvl>1: print('  REJECT trivially outside box')
            #return nan, nan, nan, nan
            return None, None, None, None

        #non-trivial case, at least one point outside window
        # this is not a bitwise or, it's the word "or"
        opt = k1 or k2 # take first non-zero point, short circuit logic
        if opt & UPPER: #these are bitwise ANDS
            x = x1 + (x2 - x1) * (ymax - y1) / (y2 - y1)
            y = ymax
        elif opt & LOWER:
            x = x1 + (x2 - x1) * (ymin - y1) / (y2 - y1)
            y = ymin
        elif opt & RIGHT:
            y = y1 + (y2 - y1) * (xmax - x1) / (x2 - x1)
            x = xmax
        elif opt & LEFT:
            y = y1 + (y2 - y1) * (xmin - x1) / (x2 - x1)
            x = xmin
        else:
            raise RuntimeError('Undefined clipping state')

        if opt == k1:
            x1, y1 = x, y
            k1 = _getclip(x1, y1,xmin,xmax,ymin,ymax)
            #if dbglvl>1: print('checking k1: ' + str(x) + ',' + str(y) + '    ' + str(k1))
        elif opt == k2:
            #if dbglvl>1: print('checking k2: ' + str(x) + ',' + str(y) + '    ' + str(k2))
            x2, y2 = x, y
            k2 = _getclip(x2, y2,xmin,xmax,ymin,ymax)
    return x1, y1, x2, y2
    
def _getclip(xa, ya,xmin,xmax,ymin,ymax):
        #if dbglvl>1: print('point: '),; print(xa,ya)
    p = INSIDE  #default is inside

        # consider x
    if xa < xmin:
        p |= LEFT
    elif xa > xmax:
        p |= RIGHT
        # consider y
    if ya < ymin:
        p |= LOWER # bitwise OR
    elif ya > ymax:
        p |= UPPER #bitwise OR
    return p
    
    
    

    
def Loop():
    robot = VectorAddingAvoidance(10,30,200,500)
    goal = Target(850,200)
    obstacle1 = obstacle(500,250,20,20)
    obstacle2 = obstacle(650,250,90,50)
    obstacle3 = obstacle(700,300,60,60)
    obstacle4 = obstacle(650,350,30,20)
    #angle = 0 
    #Vector = (5,80)
    robot.Rotate(0)
    ObstaclesList = [obstacle1,obstacle2,obstacle3,obstacle4]
    #ObstaclesList = [] #no obstacles
    print(robot.VectorAvoidance(ObstaclesList,goal))
    while True:
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                pygame.quit()
        robot.Movement(robot.WayPointVector(goal))
        #robot.Movement(robot.WayPointVector(goal))
        gameDisplay.fill(white)
        robot.LidarScan(ObstaclesList)
        #print((robot.ObstacleVector([obstacle1,obstacle2])))
        robot.Draw()
        for Obs in ObstaclesList:
            Obs.Draw()
        goal.Draw()
        pygame.display.update()
        clock.tick(FPS)
Loop()
