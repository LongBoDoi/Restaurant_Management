interface PagingData<MLEntity> {
    Data: MLEntity[],
    TotalCount: number
}

export default PagingData;