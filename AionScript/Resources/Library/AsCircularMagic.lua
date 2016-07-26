-- Enable smooth casting feature.
function EnableSmoothCasting()
	Player:UpdateGravityLock( 500 );
end
-- Disable smooth casting feature.
function DisableSmoothCasting()
	Player:UpdateGravityLock( 0 );
end
-- Initialize the Circular Teleportation Magic.
function DoInitialize( Position, Distance, Line )
	-- Initialize the position list array.
	local CurrentPositionList = {};
	-- Initialize the position list array iterator.
	self.PositionListNumber = 2;
	-- Check if a line distance is available.
	if Line ~= nil then
		-- Calculate the number of points per section.
		self.NumberOfPointsPerSection = math.floor( Line / Distance );
		-- Iterate through the first section.
		for i = 1, self.NumberOfPointsPerSection do
			-- Retrieve the player position.
			local Pos = Position:Clone();
			-- Change the X-axis of the position.
			Pos.X = Pos.X + Distance * i * math.sin( Player:GetCamera().X / 180 * math.pi );
			-- Change the Y-axis of the position.
			Pos.Y = Pos.Y - Distance * i * math.cos( Player:GetCamera().X / 180 * math.pi );
			-- Save the position.
			CurrentPositionList[i] = Pos;
		end
		-- Iterate through the second and third section.
		for i = 1, self.NumberOfPointsPerSection * 2 do
			-- Retrieve the player position.
			local Pos = CurrentPositionList[self.NumberOfPointsPerSection]:Clone();
			-- Change the X-axis of the position.
			Pos.X = Pos.X - Distance * i * math.sin( Player:GetCamera().X / 180 * math.pi );
			-- Change the Y-axis of the position.
			Pos.Y = Pos.Y + Distance * i * math.cos( Player:GetCamera().X / 180 * math.pi );
			-- Save the position.
			CurrentPositionList[self.NumberOfPointsPerSection + i] = Pos;
		end
		-- Iterate through the second and fourth section.
		for i = 1, self.NumberOfPointsPerSection do
			-- Retrieve the player position.
			local Pos = CurrentPositionList[self.NumberOfPointsPerSection * 3]:Clone();
			-- Change the X-axis of the position.
			Pos.X = Pos.X + Distance * i * math.sin( Player:GetCamera().X / 180 * math.pi );
			-- Change the Y-axis of the position.
			Pos.Y = Pos.Y - Distance * i * math.cos( Player:GetCamera().X / 180 * math.pi );
			-- Save the position.
			CurrentPositionList[self.NumberOfPointsPerSection * 3 + i] = Pos;
		end
	-- Otherwise use circular movement.
	else
		-- Loop through 36 points, each point is 10 degrees.
		for i = 1, 36 do
			-- Retrieve the player position.
			local Pos = Position:Clone();
			-- Change the X-axis of the position.
			Pos.X = Pos.X + Distance * math.sin(( i - 1 ) * 10 / 180 * math.pi );
			-- Change the Y-axis of the position.
			Pos.Y = Pos.Y - Distance * math.cos(( i - 1 ) * 10 / 180 * math.pi );
			-- Save the position.
			CurrentPositionList[i] = Pos;
		end
		-- Set the number of points per section.
		self.NumberOfPointsPerSection = 36;
	end
	-- Set the position list array.
	self.PositionList = CurrentPositionList;
	-- Update the teleport time.
	self.PositionTeleportTime = Time();
end
-- Move the Circular Teleportation Magic.
function DoMove()
	-- Check if nothing is being casted and the movement time has expired.
	if not Player:IsBusy() and Player:GetSkillTime() == 0 and not DialogList:GetDialog( "skill_delay_dialog" ):Update():IsVisible() and not DialogList:GetDialog( "enchant_delay_dialog" ):Update():IsVisible() then
		-- Retrieve the entity of the spirit.
		local EntitySpirit = self:DoSpirit( Player:GetID());
		-- Calculate the required amount of teleport position changes.
		local TeleportNumber = math.ceil(( Time() - self.PositionTeleportTime ) / 1000 );
		-- Check if the amount of teleport position changes exceeds the maximum.
		if TeleportNumber > self.NumberOfPointsPerSection / 2 then
			TeleportNumber = self.NumberOfPointsPerSection / 2;
		end
		-- Loop through the required amount of teleport position changes.
		for i = 0, TeleportNumber, 1 do
			-- Check if the end of the position list has been reached.
			if self.PositionListNumber == table.getn( self.PositionList ) then
				self.PositionListNumber = 1;
			-- Otherwise increment the position.
			else
				self.PositionListNumber = self.PositionListNumber + 1;
			end
		end
		-- Update the Z-axis with the current Z-axis, to allow playing with this.
		self.PositionList[self.PositionListNumber].Z = Player:GetPosition().Z;
		-- Set the correct position.
		Player:SetPosition( self.PositionList[self.PositionListNumber] );
		-- Set the correct spirit position.
		if EntitySpirit ~= nil then
			EntitySpirit:SetPosition( self.PositionList[self.PositionListNumber] );
		end
		-- Update the teleport time.
		self.PositionTeleportTime = Time();
		-- Teleportation was a success.
		return true;
	end
	-- Teleportation was not a success.
	return false;
end
-- Find the spirit.
function DoSpirit( ID )
	-- Loop through the available entities to find the spirit.
	for EntityID, Entity in DictionaryIterator( EntityList:GetList()) do
		-- Check if this monster is a spirit and belongs to me!
		if Entity:GetOwnerID() == ID and string.find( Entity:GetName(), "Spirit" ) ~= nil then
			return Entity;	
		end
	end
	-- Return nothing, the spirit has not been found.
	return nil;
end