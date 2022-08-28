export const routeServersIndex = '/servers'
export function routeToServer(serverId: string) {
    return `${routeServersIndex}/${encodeURIComponent(serverId)}`
}