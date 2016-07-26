luanet.load_assembly( "System.Numerics" )
Complex = luanet.import_type( "System.Numerics.Complex" )

function OnLoad()

	switcher = 10;
	
	if switcher == 1 then -- inventory check
		
		Write( "number of pots: " .. InventoryList:GetInventory( "Greater Mana Elixir"):GetCooldown() )
	
	elseif switcher == 2 then -- State check
		
		for j = 0 , Player:GetState():GetStateSize() - 1 , 1 do
			local test = Player:GetState():GetStateIndex(j)
			Write(test:GetName())
		end
	
	elseif switcher == 3 then -- Statelist check
	
		if StateList:GetList():GetState( "Mana Treatment") ~= nil then
			Write( "yay")
		else
			Write( "bah")
		end
	
	elseif switcher == 4 then -- rename characters
	
		EntityList["Mewtwo"]:SetName("TheyRot");
		EntityList["Lillia"]:SetName("Blastradius");
	
	elseif switcher == 5 then -- Position computation using 2 entities
	
			Position( Player, EntityList:GetEntity( Player:GetTargetID()) , 15 )

	elseif swticher == 6 then -- Boss positioning script
	
		BossPositioning(Masters, EntityList:GetEntity( "Blastradius"), EntityList:GetEntity( Player:GetTargetID()), 3, 20, 1 )
	
	elseif switcher == 7 then -- food test

		number = 9959;
		
		if Player:GetState():GetState( number ) ~= nil then
			Write( "0: on");
		else
			Write( "0: not on");
		end
		
		if not (Player:GetState():GetState( number ) ~= nil) then
			Write( "1: not on");
		else
			Write( "1: on");
		end
		
		if Player:GetState():GetState( number ) == nil then
			Write( "2: not on");
		else
			Write( "2: on");
		end
		Close()
		
	elseif switcher == 8 then
	
		stop_timer = {1 , 0 , 0}; -- { hours , minutes , seconds }
		ShutdownTime = Time() + 1000*( stop_timer[3] + 60*( stop_timer[2] + 60* stop_timer[1] ));

	elseif switcher == 9 then
	
		CheckTeleportSkill( EntityList:GetEntity( Player:GetTargetID() ) )
	
	elseif switcher == 10 then -- Read HTML dialog number
		Write( DialogList:GetDialog( "dlg_dialog/html_view" ):GetHTML() )
	end
	
end

function OnRun()

	if switcher == 8 then -- auto shutdown test
	
		if Time() > ShutdownTime and stop_timer[1] + stop_timer[2] + stop_timer[3] > 0 then
			PlayerInput:Console("/quit");
		end
		
	end
	
end

-- returns true when teleporting is possible
function CheckTeleportSkill(Target, aggro_distance)

	Write( Target:GetName() );

	local distance = 0;

	-- Check which skill is triggered, if any is triggered
	if CheckAvailable( "Blind Leap" ) then
		distance = 15;
	elseif CheckAvailable( "Retreating Slash" ) or CheckAvailable( "Fighting Withdrawal") then
		distance = 25;
	else
		return false;
	end
	
	-- Determine rotation and add pi for away teleportation
	local x = Player:GetPosition().x - Target:GetPosition().x;
	local y = Player:GetPosition().y - Target:GetPosition().y;
	local r = math.sqrt( x*x + y*y);

	local Angle = ( Complex.Log( Complex( x/r, y/r ) ) ).Imaginary;

	-- Determine teleport position
	local Pos = Player:GetPosition();
			
	Pos.x = Player:GetPosition().x + distance * math.sin( Angle + math.pi / 2 ) -- why the f isnt this a cos
	Pos.y = Player:GetPosition().y - distance * math.cos( Angle + math.pi / 2 )
	-- z remains. Does not matter.

	-- Player:SetMove( Pos ) -- test function
	
	local List = EntityList:GetList()
	
	for ID, Entity in DictionaryIterator( List ) do
	
		-- half way point mob distance
		pos1_distance = math.sqrt( ( (Pos.x + Player:GetPosition().x)/2 - Entity:GetPosition().x )^2 + ( (Pos.y + Player:GetPosition().y)/2 - Entity:GetPosition().y )^2 + ( (Pos.z + Player:GetPosition().z)/2 - Entity:GetPosition().z )^2 );
		-- end point mob distance
		pos2_distance = math.sqrt( ( Pos.x - Entity:GetPosition().x )^2 + ( Pos.y - Entity:GetPosition().y )^2 + ( Pos.z - Entity:GetPosition().z )^2 );
	
		if Entity:IsMonster() and Entity:IsHostile() and ( pos1_distance < aggro_distance or pos2_distance < aggro_distance ) then
			cannot_teleport = true;
		end
	end
	
	if cannot_teleport ~= nil then
		return false;
	else
		return true;
	end
	
end

function Distance( Entity1, Entity2 )

	return Entity1:GetPosition():DistanceToPosition( Entity2:GetPosition());
	
end

--
-- function Position( Master, Target, distance ), with
-- Master = array of Entities, These Entities are following the Target.
-- Target = Entity, The one you are following
-- distance, integer. Distance to be from Target, in units.
function Position( Masters, Target, distance )

	-- Check is player is master
	local PlayerIsMaster = false;
	local j = 0;
	if #Masters > 0 then
			
		for i = 1,#Masters,1 do
					
			if Master[i]:GetID() == Player:GetID() then

				PlayerIsMaster = true;
				j = i;
				
			end
		end
		
	else
		return true
	end

	-- Only move when you are the master (follower) and you are out of desired range
	if PlayerIsMaster and ( Distance( Master, Target) < distance - 2 or Distance( Master, Target) > distance + 2 ) then
	
		-- differential x,y,z, and r computations
		local x = Target:GetPosition().x - Masters[j]:GetPosition().x;
		local y = Target:GetPosition().y - Masters[j]:GetPosition().y;
		local z = Target:GetPosition().z - Masters[j]:GetPosition().z;
		local r = math.sqrt( x*x + y*y);
		
		-- Angle calculations
		local theta = ( Complex.Log( Complex( y/r, -x/r ) ) ).Imaginary;
		local phi = math.atan( z / r );

		-- New position calculation
		local Pos = Player:GetPosition()
		Pos.x = Target:GetPosition().x + distance * math.sin( theta ) * math.cos( phi )
		Pos.y = Target:GetPosition().y - distance * math.cos( theta ) * math.cos( phi )
		Pos.z = Target:GetPosition().z + distance * math.sin( phi )
		
		-- Movement		
		Player:SetMove( Pos )
		
		return false
	end
	
	return true
	
end

--
-- function BossPositioning( Masters, Tank, Target,  TankDistance, MasterDistance, Away) with
-- Masters = array of Entities, 
-- Tank, Target = Entity,
-- TankDistance, MasterDistance = double.
-- Away = boolean, away = 1 means turn boss away, 0 = stay on same side as master
-- returns true when finished
function BossPositioning( Masters, Tank, Target,  TankDistance, MasterDistance, Away)
	
	-- Solve problems with negative numbers and too small distances.
	if TankDistance < 3 then
		TankDistance = 3;
	end
	if MasterDistance < 3 then
		MasterDistance = 3;
	end

	-- Tolerances
	local AngleTolerance = math.pi / 18; 	-- Angle tolerance between masters and tank
	local TankDistanceTolerance = 5; 		-- Tolerance before tank starts moving towards Target
	local MasterDistanceTolerance = 3;		-- Tolerance in distance before Master start moving to correct position
	
	local MasterAngle = 0 	-- Create Master Angle variable
	local j = 0 			-- count number of masters

	if #Masters > 0 then
			
		for i = 1,#Masters,1 do
					
			local x = Target:GetPosition().x - Masters[i]:GetPosition().x;
			local y = Target:GetPosition().y - Masters[i]:GetPosition().y;
			local r = math.sqrt( x*x + y*y);
			local angle = ( Complex.Log( Complex( x/r, y/r ) ) ).Imaginary - math.pi/2;
			
			MasterAngle = MasterAngle + angle;
			j = i

		end
		MasterAngle = MasterAngle / j; -- average angle over all masters

	else
		Write( "number of masters in BossPositioning is incorrect")
	end
		
	-- Tank gets aggro
	if Target:GetTargetID() ~= Tank:GetID() and Distance( Tank, Target) > TankDistanceTolerance then

		if Player:GetID() == Tank:GetID() then

			local pos = Player:GetPosition()	

			pos.x = Target:GetPosition().x + TankDistance * math.sin( MasterAngle + Away * math.pi )
			pos.y = Target:GetPosition().y - TankDistance * math.cos( MasterAngle + Away * math.pi )
			-- z remains. Does not matter.
			
			Player:SetMove(pos)

		end

		return false
	
	-- Tank is taking (moving) or has taken position. Allow attack routine to kick in.
	elseif Player:GetID() == Tank:GetID() then

		local x = Target:GetPosition().x - Tank:GetPosition().x;
		local y = Target:GetPosition().y - Tank:GetPosition().y;
		local r = math.sqrt( x*x + y*y);

		local TankAngle = ( Complex.Log( Complex( x/r, y/r ) ) ).Imaginary + Away * math.pi; -- add away rotation to verify their angles

		if math.abs( math.mod( TankAngle + 2 * math.pi, 2*math.pi) - math.mod(MasterAngle + 2 * math.pi, 2 * math.pi ) ) < AngleTolerance then
		
			return true
			
		else
			
			local Pos = Player:GetPosition();
			
			Pos.x = Target:GetPosition().x + TankDistance * math.sin( MasterAngle + Away * math.pi )
			Pos.y = Target:GetPosition().y - TankDistance * math.cos( MasterAngle + Away * math.pi )
			-- z remains. Does not matter.
				
			Player:SetMove( Pos )
		
			return false
			
		end
		
	-- Tank takes position, minimum distance = 5, safety distance
	elseif Tank:GetSkillID() ~= 0 and SkillList:GetSkill( Tank:GetSkillID() ):IsAttack() and Distance( Tank, Target) < TankDistanceTolerance and ( Distance( Player, Target) < MasterDistance - MasterDistanceTolerance or Distance( Player, Target) > MasterDistance + MasterDistanceTolerance ) then

		local Pos = Player:GetPosition()
			
		Pos.x = Target:GetPosition().x + MasterDistance * math.sin( MasterAngle )
		Pos.y = Target:GetPosition().y - MasterDistance * math.cos( MasterAngle )
		-- z remains. Does not matter.
					 
		Player:SetMove( Pos )
		
		return false
		
	else

		return true
	
	end
	
end


function CheckAvailable( Name, SkipActivation )

	-- Retrieve the ability with the provided name.
	local Ability = AbilityList:GetAbility( Name );

	-- Check if the ability is valid and is not in cooldown.
	if Ability ~= nil and Ability:GetCooldown() == 0 and ( SkipActivation ~= nil or Ability:GetActivated()) then
		return true;
	end

	-- Either we do not have the ability or it is in cooldown.
	return false;
	
end

--- Check if the provided inventory is available and has cooled down.
--
-- @param	string	Name of the inventory to check.
-- @param	integer	Contains the amount required to check, instead of a valid cooldown.
-- @return	boolean

function CheckAvailableInventory( Name, Amount )

	-- Retrieve the item with the provided name.
	local Inventory = InventoryList:GetInventory( Name );

	-- Check if the item is valid and is not in cooldown.
	if Inventory ~= nil and (( Amount == nil and Inventory:GetCooldown() == 0 ) or ( Amount ~= nil and Inventory:GetAmount() >= Amount )) then
		return true;
	end

	-- Either we do not have the item or it is in cooldown.
	return false;
	
end

--- Checks if the target we have matches the conditions to cast friendly magic.
--
-- @param	Entity	Contains the entity to match the target on.
-- @param	bool	Indicates whether or not this is a hostile spell.
-- @return	bool

function CheckExecute( Name, Entity )

	-- Retrieve the ability with the provided name.
	local Ability = AbilityList:GetAbility( Name );
	
	-- Check if the provided ability is available and return when it is not.
	if Ability == nil or Ability:GetCooldown() ~= 0 then
		return false;
	end
	
	-- Check if I am currently resting and stop resting when I am!
	if Player:IsResting() then
		PlayerInput:Ability( "Toggle Rest" );
		return false;
	end
	
	-- Check if this is a friendly ability with my own character as the target.
	if Entity ~= nil and Player:GetID() == Entity:GetID() then
		
		-- Retrieve the skill based on the ability identifier.
		local Skill = SkillList:GetSkill( Ability:GetID());
	
		-- It is not possible to perform a hostile skill on my own character.
		if Skill == nil or Skill:IsAttack() then
			return false;
		end
		
		-- When no target has been selected we can execute the ability.
		if Player:GetTargetID() == 0 then
			return PlayerInput:Ability( Name );
		end
		
		-- Otherwise retrieve the entity we currently have selected.
		local EntityTarget = EntityList:GetEntity( Player:GetTargetID());
		
		-- When the target is valid and is not friendly we can use our ability.
		if EntityTarget ~= nil and not EntityTarget:IsFriendly() then
			return PlayerInput:Ability( Name );
		end
		
	end

	-- Check if the target entity has been selected and select it when it is needed.
	if Entity ~= nil and Player:GetTargetID() ~= Entity:GetID() then
		Player:SetTarget( Entity );
		return false;
	end
	
	-- Everything seems to be valid 
	return PlayerInput:Ability( Name );
	
end

--- Checks if the provided Entity has one of the known reflection-based states.
--
-- @param	Entity	Contains the entity to check for reflect.
-- @return	bool

function CheckMelee()

	-- Retrieve the class of the current character.
	local Class = Player:GetClass():ToString();
	
	-- Check if the class is a focused melee-orientated class.
	if Class == "Gladiator" or Class == "Templar" or Class == "Assassin" or Class == "Chanter" then
		return true;
	end
	
	-- Otherwise return false, since we are a magical-based class.
	return false;
	
end

--- Check if a mana treatment can be executed and execute it when required.
--
-- @return	boolean

function CheckManaTreatment()

	-- Retrieve the total mana recharge required.
	local TotalRecharge = Player:GetManaMaximum() - Player:GetManaCurrent();
	
	if Settings.ManaTreatmentTreshhold == 0 or Player:GetMana() < Settings.ManaTreatmentTreshhold then
		if self:CheckAvailable( "Mana Treatment IV" ) and TotalRecharge >= 648 and self:CheckAvailableInventory( "Fine Odella Powder", 2 ) then
			self:CheckExecute( "Mana Treatment IV" );
			return false;
		elseif self:CheckAvailable( "Mana Treatment III" ) and TotalRecharge >= 535 and self:CheckAvailableInventory( "Greater Odella Powder", 2 ) then
			self:CheckExecute( "Mana Treatment III" );
			return false;
		elseif self:CheckAvailable( "Mana Treatment II" ) and TotalRecharge >= 404 and self:CheckAvailableInventory( "Odella Powder", 2 ) then
			self:CheckExecute( "Mana Treatment II" );
			return false;
		elseif self:CheckAvailable( "Mana Treatment I" ) and TotalRecharge >= 298 and self:CheckAvailableInventory( "Lesser Odella Powder", 2 ) then
			self:CheckExecute( "Mana Treatment I" );
			return false;
		end
	end
	
	-- Return true when nothing is executed.
	return true;
	
end

--- Check if the provided ability is available and return the full name.
--
-- @param	string	Name of the ability to check.
-- @return	boolean

function CheckName( zName )

	-- Get the ability with the provided name.
	local Ability = AbilityList:GetAbility( zName );
	
	-- Check if the ability is valid (you have learned it) and 
	if Ability ~= nil then
		return Ability:GetName();
	end
	
	-- We do not have the ability, return nothing.
	return nil;
	
end

--- Checks the best potion required for the current level of the player mana and uses it.
--
-- @return	boolean

function CheckPotionMana()

	-- Prepare the variables to contain the best item and recharge.
	local BestInventory = nil;
	local BestRecharge = 0;
	local TotalRecharge = Player:GetManaMaximum() - Player:GetManaCurrent();
	
	-- Check if a potion is available.
	if self._iPotionDelay == nil or self._iPotionDelay < Time() then
	
		-- Loop through your inventory.
		for Inventory in ListIterator( InventoryList:GetList()) do

			-- Check if this is a mana elixer or mana potion.
			if Inventory:GetType():ToString() == "Potion" and ( string.find( Inventory:GetName(), "Mana Elixir" ) ~= nil or string.find( Inventory:GetName(), "Mana Potion" ) ~= nil ) then
				
				if string.find( Inventory:GetName(), "Fine" ) ~= nil then
					if TotalRecharge >= 1830 and BestRecharge < 1830 then
						BestInventory = Inventory;
						BestRecharge = 1830; -- 1530;
					end
				elseif string.find( Inventory:GetName(), "Major" ) ~= nil then
					if TotalRecharge >= 1600 and BestRecharge < 1600 then
						BestInventory = Inventory;
						BestRecharge = 1600; -- 1340;
					end
				elseif string.find( Inventory:GetName(), "Greater" ) ~= nil then
					if TotalRecharge >= 1480 and BestRecharge < 1480 then
						BestInventory = Inventory;
						BestRecharge = 1480; -- 1240;
					end
				elseif string.find( Inventory:GetName(), "Lesser" ) ~= nil then
					if TotalRecharge >= 950 and BestRecharge < 950 then
						BestInventory = Inventory;
						BestRecharge = 950; -- 820;
					end
				elseif string.find( Inventory:GetName(), "Minor" ) ~= nil then
					if TotalRecharge >= 590 and BestRecharge < 590 then
						BestInventory = Inventory;
						BestRecharge = 590; -- 500;
					end
				elseif TotalRecharge >= 1280 and BestRecharge < 1280 then
					BestInventory = Inventory;
					BestRecharge = 1280; -- 1070;
				end
				
			end

		end
		
		-- Check if we have a positive match and see if the cooldown allows the use of it.
		if BestInventory ~= nil and BestInventory:GetCooldown() == 0 then
			PlayerInput:Inventory( BestInventory:GetName());
			self._iPotionDelay = Time() + BestInventory:GetReuse();
			return false;
		end
		
	end
	
	-- We have not executed any potion.
	return true;
	
end

--- Checks the best potion required for the current level of the player health and uses it.
--
-- @return	boolean

function CheckPotionHealth()

	-- Prepare the variables to contain the best item and recharge.
	local BestInventory = nil;
	local BestRecharge = 0;
	local TotalRecharge = Player:GetHealthMaximum() - Player:GetHealthCurrent();
	
	-- Check if a potion is available.
	if self._iPotionDelay == nil or self._iPotionDelay < Time() then
	
		-- Loop through your inventory
		for Inventory in ListIterator( InventoryList:GetList()) do

			-- Check if this is a life potion
			if Inventory:GetType():ToString() == "Potion" and ( string.find( Inventory:GetName(), "Life Elixir" ) ~= nil or string.find( Inventory:GetName(), "Life Potion" ) ~= nil ) then
				
				if string.find( Inventory:GetName(), "Fine" ) ~= nil then
					if TotalRecharge >= 2120 and BestRecharge < 2120 then
						BestInventory = Inventory;
						BestRecharge = 2120;
					end
				elseif string.find( Inventory:GetName(), "Major" ) ~= nil then
					if TotalRecharge >= 1540 and BestRecharge < 1540 then
						BestInventory = Inventory;
						BestRecharge = 1540;
					end
				elseif string.find( Inventory:GetName(), "Greater" ) ~= nil then
					if TotalRecharge >= 1270 and BestRecharge < 1270 then
						BestInventory = Inventory;
						BestRecharge = 1270;
					end
				elseif string.find( Inventory:GetName(), "Lesser" ) ~= nil then
					if TotalRecharge >= 670 and BestRecharge < 670 then
						BestInventory = Inventory;
						BestRecharge = 670;
					end
				elseif string.find( Inventory:GetName(), "Minor" ) ~= nil then
					if TotalRecharge >= 370 and BestRecharge < 370 then
						BestInventory = Inventory;
						BestRecharge = 370;
					end
				elseif TotalRecharge >= 970 and BestRecharge < 970 then
					BestInventory = Inventory;
					BestRecharge = 970;
				end
				
			end

		end
		
		-- Check if we have a positive match and see if the cooldown allows the use of it.
		if BestInventory ~= nil and BestInventory:GetCooldown() == 0 then
			PlayerInput:Inventory( BestInventory:GetName());
			self._iPotionDelay = Time() + BestInventory:GetReuse();
			return false;
		end
	
	end
	
	-- We have not executed any potion.
	return true;
	
end

--- Checks the best potion required for the current level of the player health/mana and uses it.
--
-- @return	boolean

function CheckPotionRecovery()

	-- Prepare the variables to contain the best item and recharge.
	local BestInventory = nil;
	local BestRecharge = 0;
	local TotalRechargeHealth = Player:GetHealthMaximum() - Player:GetHealthCurrent();
	local TotalRechargeMana = Player:GetManaMaximum() - Player:GetManaCurrent();
	
	-- Check if a potion is available.
	if self._iPotionDelay == nil or self._iPotionDelay < Time() then
		
		-- Loop through your inventory
		for Inventory in ListIterator( InventoryList:GetList()) do

			-- Check if this is a life potion
			if Inventory:GetType():ToString() == "Potion" and string.find( Inventory:GetName(), "Recovery Potion" ) ~= nil then
				
				if string.find( Inventory:GetName(), "Fine" ) ~= nil then
					if TotalRechargeHealth >= 2120 and TotalRechargeMana >= 1830 and BestRecharge < 2120 then
						BestInventory = Inventory;
						BestRecharge = 2120;
					end
				elseif string.find( Inventory:GetName(), "Major" ) ~= nil then
					if TotalRechargeHealth >= 1540 and TotalRechargeMana >= 1600 and BestRecharge < 1540 then
						BestInventory = Inventory;
						BestRecharge = 1540;
					end
				elseif string.find( Inventory:GetName(), "Greater" ) ~= nil then
					if TotalRechargeHealth >= 1270 and TotalRechargeMana >= 1480 and BestRecharge < 1270 then
						BestInventory = Inventory;
						BestRecharge = 1270;
					end
				elseif string.find( Inventory:GetName(), "Lesser" ) ~= nil then
					if TotalRechargeHealth >= 670 and TotalRechargeMana >= 980 and BestRecharge < 670 then
						BestInventory = Inventory;
						BestRecharge = 670;
					end
				elseif string.find( Inventory:GetName(), "Minor" ) ~= nil then
					if TotalRechargeHealth >= 370 and TotalRechargeMana >= 590 and BestRecharge < 370 then
						BestInventory = Inventory;
						BestRecharge = 370;
					end
				elseif TotalRechargeHealth >= 970 and TotalRechargeMana >= 1280 and BestRecharge < 970 then
					BestInventory = Inventory;
					BestRecharge = 970;
				end
				
			end

		end
		
		-- Check if we have a positive match and see if the cooldown allows the use of it.
		if BestInventory ~= nil and BestInventory:GetCooldown() == 0 then
			PlayerInput:Inventory( BestInventory:GetName());
			self._iPotionDelay = Time() + BestInventory:GetReuse();
			return false;
		end
		
	end
	
	-- We have not executed any potion.
	return true;
	
end

--- Checks if the provided Entity has one of the known reflection-based states.
--
-- @param	Entity	Contains the entity to check for reflect.
-- @return	bool

function CheckReflect( Entity )

	-- Check if the provided entity is valid.
	if Entity == nil then
		return false;
	end 
	
	-- Retrieve the state for this entity to check for known reflect states.
	local EntityState = Entity:GetState();
	
	-- Check if the retrieved entity state is valid.
	if EntityState == nil then
		return false;
	end
	
	-- Check the list of known reflection states.
	if EntityState:GetState( "Stigma: Protection" ) ~= nil	-- Nexus (Udas Temple)
		or EntityState:GetState( "Punishment" ) ~= nil then	-- The Soulcaller (Beshmundirs Temple)
		return true;
	end
	
	-- None of the known reflection 
	return false;
	
end
