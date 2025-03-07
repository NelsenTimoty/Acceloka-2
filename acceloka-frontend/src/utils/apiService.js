const BASE_URL = "http://localhost:5297/api/v1"; // Change if needed

// Fetch booked tickets
export const getBookedTickets = async () => {
  const response = await fetch(`${BASE_URL}/booked-tickets`);
  return response.json();
};

// Delete a booked ticket
export const deleteBookedTicket = async (bookedTicketId, ticketCode, quantity) => {
  const response = await fetch(`${BASE_URL}/booked-tickets/${bookedTicketId}/${ticketCode}/${quantity}`, {
    method: "DELETE",
  });
  return response.json();
};

// Update a booked ticket
export const updateBookedTicket = async (bookedTicketId, updatedData) => {
  const response = await fetch(`${BASE_URL}/booked-tickets/${bookedTicketId}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(updatedData),
  });
  return response.json();
};

// Book a ticket
export const bookTicket = async (ticketData) => {
  const response = await fetch(`${BASE_URL}/book-ticket`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(ticketData),
  });
  return response.json();
};

// Fetch available tickets
export const getAvailableTickets = async () => {
  const response = await fetch(`${BASE_URL}/get-available-ticket`);
  return response.json();
};
