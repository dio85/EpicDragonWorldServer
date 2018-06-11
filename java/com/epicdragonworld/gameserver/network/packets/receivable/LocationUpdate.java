/*
 * This file is part of the Epic Dragon World project.
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
package com.epicdragonworld.gameserver.network.packets.receivable;

import com.epicdragonworld.gameserver.managers.WorldManager;
import com.epicdragonworld.gameserver.model.WorldObject;
import com.epicdragonworld.gameserver.model.actor.instance.PlayerInstance;
import com.epicdragonworld.gameserver.network.GameClient;
import com.epicdragonworld.gameserver.network.ReceivablePacket;
import com.epicdragonworld.gameserver.network.packets.sendable.MoveToLocation;

/**
 * @author Pantelis Andrianakis
 */
public class LocationUpdate
{
	public LocationUpdate(GameClient client, ReceivablePacket packet)
	{
		// Read data.
		final float posX = (float) packet.readDouble(); // TODO: Client WriteFloat
		final float posY = (float) packet.readDouble(); // TODO: Client WriteFloat
		final float posZ = (float) packet.readDouble(); // TODO: Client WriteFloat
		
		// Update player location.
		final PlayerInstance player = client.getActiveChar();
		if (player != null)
		{
			player.getLocation().setX(posX);
			player.getLocation().setY(posY);
			player.getLocation().setZ(posZ);
			
			// Broadcast movement.
			for (WorldObject object : WorldManager.getInstance().getVisibleObjects(player))
			{
				if (object.isPlayer())
				{
					((PlayerInstance) object).channelSend(new MoveToLocation(player));
				}
				// TODO: Other objects.
			}
		}
	}
}
